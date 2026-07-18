using BuildingBlocks.Contracts.Events.Payment;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Infrastructure.Entities;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Interfaces.IServices;

namespace Payment.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentsController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IVnpayService vnpayService,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ILogger<PaymentsController> logger)
        {
            _vnpayService = vnpayService;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [HttpGet("vnpay-ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayIpn()
        {
            try
            {
                Dictionary<string, string> vnpayParams = new Dictionary<string, string>();

                string secureHash = string.Empty;
                foreach (var key in Request.Query.Keys)
                {
                    string value = Request.Query[key]!;

                    if (key == "vnp_SecureHash")
                    {
                        secureHash = value;
                    }
                    else if (key.StartsWith("vnp_"))
                    {
                        vnpayParams.Add(key, value);
                    }
                }

                bool isValid = _vnpayService.ValidateSignature(vnpayParams, secureHash);

                if (!isValid)
                {
                    _logger.LogWarning("VNPay IPN: Invalid signature");
                    return BadRequest(new { RspCode = "97", Message = "Invalid signature" });
                }

                string txnRef = Request.Query["vnp_TxnRef"]!;
                var orderId = Guid.Parse(txnRef);
                var transaction = await _unitOfWork.PaymentTransactionRepository.GetOneAsync<PaymentTransaction>(
                    filter: t => t.OrderId == orderId
                );

                if (transaction == null)
                {
                    _logger.LogWarning("VNPay IPN: Order not found for TxnRef {TxnRef}", txnRef);
                    return NotFound(new { RspCode = "01", Message = "Order not found" });
                }
                if (transaction.Status != PaymentStatus.Pending)
                {
                    return Ok(new { RspCode = "02", Message = "Order already confirmed" });
                }

                string responseCode = Request.Query["vnp_ResponseCode"]!;
                string transactionStatus = Request.Query["vnp_TransactionStatus"]!;
                string vnpayTranId = Request.Query["vnp_TransactionNo"]!;
                string rawResponse = string.Join("&", Request.Query.Select(q => $"{q.Key}={q.Value}"));

                if (responseCode == "00" && transactionStatus == "00")
                {
                    transaction.MarkAsSuccess(vnpayTranId, rawResponse);
                    await _unitOfWork.PaymentTransactionRepository.UpdateAsync(transaction);
                    await _unitOfWork.SaveChangesAsync();

                    await _publishEndpoint.Publish(new PaymentCompletedEvent
                    {
                        OrderId = transaction.OrderId,
                        TransactionId = vnpayTranId,
                        Amount = transaction.Amount,
                        Gateway = transaction.Gateway,
                        CompletedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    transaction.MarkAsFailed(vnpayTranId, rawResponse);
                    await _unitOfWork.PaymentTransactionRepository.UpdateAsync(transaction);
                    await _unitOfWork.SaveChangesAsync();
                    await _publishEndpoint.Publish(new PaymentFailedEvent
                    {
                        OrderId = transaction.OrderId,
                        Reason = $"Thanh toán thất bại tại VNPay. Mã lỗi: {responseCode}",
                        CompletedAt = DateTime.UtcNow
                    });
                }

                await _unitOfWork.SaveChangesAsync();

                return Ok(new { RspCode = "00", Message = "Confirm success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình xử lý VNPay IPN");
                return StatusCode(500, new { RspCode = "99", Message = "Unknown error" });
            }
        }

        [HttpGet("vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayReturn()
        {
            try
            {
                Dictionary<string, string> vnpayParams = new Dictionary<string, string>();
                string secureHash = string.Empty;

                foreach (var key in Request.Query.Keys)
                {
                    string value = Request.Query[key]!;

                    if (key == "vnp_SecureHash")
                    {
                        secureHash = value;
                    }
                    else if (key.StartsWith("vnp_"))
                    {
                        vnpayParams.Add(key, value);
                    }
                }

                bool isValid = _vnpayService.ValidateSignature(vnpayParams, secureHash);
                string responseCode = Request.Query["vnp_ResponseCode"]!;
                string txnRef = Request.Query["vnp_TxnRef"]!;
                bool success = isValid && responseCode == "00";

                if (success && Guid.TryParse(txnRef, out var orderId))
                {
                    var transaction = await _unitOfWork.PaymentTransactionRepository.GetOneAsync<PaymentTransaction>(
                        filter: t => t.OrderId == orderId
                    );

                    if (transaction != null && transaction.Status == PaymentStatus.Pending)
                    {
                        string vnpayTranId = Request.Query["vnp_TransactionNo"]!;
                        string rawResponse = string.Join("&", Request.Query.Select(q => $"{q.Key}={q.Value}"));

                        transaction.MarkAsSuccess(vnpayTranId, rawResponse);
                        await _unitOfWork.PaymentTransactionRepository.UpdateAsync(transaction);
                        await _unitOfWork.SaveChangesAsync();

                        await _publishEndpoint.Publish(new PaymentCompletedEvent
                        {
                            OrderId = transaction.OrderId,
                            TransactionId = vnpayTranId,
                            Amount = transaction.Amount,
                            Gateway = transaction.Gateway,
                            CompletedAt = DateTime.UtcNow
                        });

                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                string redirectUrl = $"http://localhost:5173/my-tickets?success={success.ToString().ToLower()}&orderId={txnRef}";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình xử lý VNPay Return");
                return Redirect($"http://localhost:5173/my-tickets?success=false&error=server_error");
            }
        }
    }
}
