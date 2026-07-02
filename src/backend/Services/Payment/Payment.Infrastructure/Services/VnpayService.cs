using Microsoft.Extensions.Options;
using Payment.Common.Options;
using Payment.Infrastructure.Interfaces.IServices;
using System.Security.Cryptography;
using System.Text;

namespace Payment.Infrastructure.Services
{
    public class VnpayService : IVnpayService
    {
        private readonly PaymentOptions _payment;

        public VnpayService(IOptions<PaymentOptions> payment)
        {
            _payment = payment.Value;
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
                    string keyEncoded = Uri.EscapeDataString(kvp.Key);
                    string valEncoded = Uri.EscapeDataString(kvp.Value);
                    queryBuilder.Append($"{keyEncoded}={valEncoded}&");
                    signDataBuilder.Append($"{kvp.Key}={kvp.Value}&");
                }
            }
            if (queryBuilder.Length > 0) queryBuilder.Length--;
            if (signDataBuilder.Length > 0) signDataBuilder.Length--;
            string rawData = signDataBuilder.ToString();
            string secureHash = HmacSha512(hashSecret, rawData);
            return $"{paymentUrl}?{queryBuilder}&vnp_SecureHash={secureHash}";
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
                    signDataBuilder.Append($"{kvp.Key}={kvp.Value}&");
                }
            }
            if (signDataBuilder.Length > 0) signDataBuilder.Length--;
            string calculatedHash = HmacSha512(hashSecret, signDataBuilder.ToString());
            return calculatedHash.Equals(secureHash, StringComparison.OrdinalIgnoreCase);
        }

        private string HmacSha512(string key, string inputData)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyByte))
            {
                byte[] hashMessage = hmac.ComputeHash(messageBytes);
                return Convert.ToHexString(hashMessage);
            }
        }
    }
}
