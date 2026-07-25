using Microsoft.Extensions.Options;
using Payment.Common.Options;
using Payment.Infrastructure.Interfaces.IServices;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payment.Infrastructure.Services
{
    public class VnpayService : IVnpayService
    {
        private readonly PaymentOptions _payment;
        private readonly IHttpClientFactory _httpClientFactory;

        public VnpayService(IOptions<PaymentOptions> payment, IHttpClientFactory httpClientFactory)
        {
            _payment = payment.Value;
            _httpClientFactory = httpClientFactory;
        }

        public string CreatePaymentUrl(Guid orderId, decimal amount, string ipAddress, string customerName)
        {
            string tmnCode = _payment.VNPay!.TmnCode;
            string hashSecret = _payment.VNPay!.HashSecret;
            string paymentUrl = _payment.VNPay!.PaymentUrl;
            string returnUrl = _payment.VNPay!.ReturnUrl;

            long vnpAmount = (long)(amount * 100);

            SortedDictionary<string, string> sortedParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Version", _payment.VNPay!.Version },
                { "vnp_Command", _payment.VNPay!.Command },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", vnpAmount.ToString() },
                { "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(10).ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", _payment.VNPay!.Currency },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", _payment.VNPay!.Locale },
                { "vnp_OrderInfo", $"Thanh toan don hang TicketHub: {orderId}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", orderId.ToString() }
            };

            StringBuilder signDataBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder();

            foreach (var kvp in sortedParams)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    string keyEncoded = System.Net.WebUtility.UrlEncode(kvp.Key);
                    string valEncoded = System.Net.WebUtility.UrlEncode(kvp.Value);
                    queryBuilder.Append($"{keyEncoded}={valEncoded}&");
                    signDataBuilder.Append($"{keyEncoded}={valEncoded}&");
                }
            }
            if (queryBuilder.Length > 0) queryBuilder.Length--;
            if (signDataBuilder.Length > 0) signDataBuilder.Length--;
            string rawData = signDataBuilder.ToString();
            string secureHash = HmacSha512(hashSecret, rawData);
            return $"{paymentUrl}?{queryBuilder.ToString()}&vnp_SecureHash={secureHash}";
        }

        public bool ValidateSignature(Dictionary<string, string> vnpayParams, string secureHash)
        {
            string hashSecret = _payment.VNPay!.HashSecret;

            SortedDictionary<string, string> sortedParams = new SortedDictionary<string, string>(vnpayParams, StringComparer.Ordinal);

            StringBuilder signDataBuilder = new StringBuilder();

            foreach (var kvp in sortedParams)
            {
                if (kvp.Key != "vnp_SecureHash" && kvp.Key != "vnp_SecureHashType" && !string.IsNullOrEmpty(kvp.Value))
                {
                    string keyEncoded = System.Net.WebUtility.UrlEncode(kvp.Key);
                    string valEncoded = System.Net.WebUtility.UrlEncode(kvp.Value);
                    signDataBuilder.Append($"{keyEncoded}={valEncoded}&");
                }
            }
            if (signDataBuilder.Length > 0) signDataBuilder.Length--;
            string calculatedHash = HmacSha512(hashSecret, signDataBuilder.ToString());
            return calculatedHash.Equals(secureHash, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<(bool IsSuccess, string? VnpayRefundTransactionId, string RawResponse, string Message)> RefundAsync(
            Guid orderId,
            decimal amount,
            string originalVnpTransactionNo,
            string originalPayDate,
            bool isFullRefund,
            string createdBy,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            string tmnCode = _payment.VNPay!.TmnCode;
            string hashSecret = _payment.VNPay!.HashSecret;
            string refundUrl = _payment.VNPay!.RefundUrl;

            string requestId = Guid.NewGuid().ToString("N").Substring(0, 16);
            string version = _payment.VNPay!.Version;
            string command = "refund";
            string transactionType = isFullRefund ? "02" : "03";
            string txnRef = orderId.ToString();
            long vnpAmount = (long)(amount * 100);
            string orderInfo = $"Hoan tien don hang TicketHub: {orderId}";
            string createDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");

            string signData = string.Join("|",
                requestId, version, command, tmnCode, transactionType, txnRef,
                vnpAmount.ToString(), originalVnpTransactionNo, originalPayDate,
                createdBy, createDate, ipAddress, orderInfo);

            string secureHash = HmacSha512(hashSecret, signData);

            var requestBody = new VnpayRefundRequest
            {
                RequestId = requestId,
                Version = version,
                Command = command,
                TmnCode = tmnCode,
                TransactionType = transactionType,
                TxnRef = txnRef,
                Amount = vnpAmount,
                OrderInfo = orderInfo,
                TransactionNo = originalVnpTransactionNo,
                TransactionDate = originalPayDate,
                CreateBy = createdBy,
                CreateDate = createDate,
                IpAddr = ipAddress,
                SecureHash = secureHash
            };

            HttpClient client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) TicketHub-Payment/1.0");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            using HttpResponseMessage httpResponse = await client.PostAsJsonAsync(refundUrl, requestBody, cancellationToken);
            string rawResponse = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
                return (false, null, rawResponse, $"Lỗi kết nối tới VNPay: {httpResponse.StatusCode}");

            VnpayRefundResponse? response = JsonSerializer.Deserialize<VnpayRefundResponse>(rawResponse);

            if (response == null)
                return (false, null, rawResponse, "Không đọc được phản hồi từ VNPay.");

            bool isSuccess = response.ResponseCode == "00";
            string message = isSuccess ? "Hoàn tiền thành công." : (response.Message ?? $"VNPay từ chối hoàn tiền, mã lỗi: {response.ResponseCode}");

            return (isSuccess, response.TransactionNo, rawResponse, message);
        }

        private string HmacSha512(string key, string inputData)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyByte))
            {
                byte[] hashMessage = hmac.ComputeHash(messageBytes);
                return Convert.ToHexString(hashMessage).ToLowerInvariant();
            }
        }

        private class VnpayRefundRequest
        {
            [JsonPropertyName("vnp_RequestId")]
            public string RequestId { get; set; } = default!;

            [JsonPropertyName("vnp_Version")]
            public string Version { get; set; } = default!;

            [JsonPropertyName("vnp_Command")]
            public string Command { get; set; } = default!;

            [JsonPropertyName("vnp_TmnCode")]
            public string TmnCode { get; set; } = default!;

            [JsonPropertyName("vnp_TransactionType")]
            public string TransactionType { get; set; } = default!;

            [JsonPropertyName("vnp_TxnRef")]
            public string TxnRef { get; set; } = default!;

            [JsonPropertyName("vnp_Amount")]
            public long Amount { get; set; }

            [JsonPropertyName("vnp_OrderInfo")]
            public string OrderInfo { get; set; } = default!;

            [JsonPropertyName("vnp_TransactionNo")]
            public string TransactionNo { get; set; } = default!;

            [JsonPropertyName("vnp_TransactionDate")]
            public string TransactionDate { get; set; } = default!;

            [JsonPropertyName("vnp_CreateBy")]
            public string CreateBy { get; set; } = default!;

            [JsonPropertyName("vnp_CreateDate")]
            public string CreateDate { get; set; } = default!;

            [JsonPropertyName("vnp_IpAddr")]
            public string IpAddr { get; set; } = default!;

            [JsonPropertyName("vnp_SecureHash")]
            public string SecureHash { get; set; } = default!;
        }

        private class VnpayRefundResponse
        {
            [JsonPropertyName("vnp_ResponseId")]
            public string? ResponseId { get; set; }

            [JsonPropertyName("vnp_ResponseCode")]
            public string? ResponseCode { get; set; }

            [JsonPropertyName("vnp_Message")]
            public string? Message { get; set; }

            [JsonPropertyName("vnp_TransactionNo")]
            public string? TransactionNo { get; set; }

            [JsonPropertyName("vnp_TransactionStatus")]
            public string? TransactionStatus { get; set; }
        }
    }
}
