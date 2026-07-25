namespace Payment.Infrastructure.Interfaces.IServices
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(Guid orderId, decimal amount, string ipAddress, string customerName);
        bool ValidateSignature(Dictionary<string, string> vnpayParams, string secureHash);

        Task<(bool IsSuccess, string? VnpayRefundTransactionId, string RawResponse, string Message)> RefundAsync(
            Guid orderId,
            decimal amount,
            string originalVnpTransactionNo,
            string originalPayDate,
            bool isFullRefund,
            string createdBy,
            string ipAddress,
            CancellationToken cancellationToken = default);
    }
}
