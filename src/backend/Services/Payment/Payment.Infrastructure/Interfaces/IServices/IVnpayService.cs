namespace Payment.Infrastructure.Interfaces.IServices
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(Guid orderId, decimal amount, string ipAddress, string customerName);
        bool ValidateSignature(Dictionary<string, string> vnpayParams, string secureHash);
    }
}
