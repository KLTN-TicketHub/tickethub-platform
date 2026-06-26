namespace Payment.Common.Options
{
    public class PaymentOptions
    {
        public string? DefaultProvider { get; set; }

        public VnPayOptions? VNPay { get; init; }
    }
    public class VnPayOptions
    {
        public string Version { get; init; } = default!;

        public string Command { get; init; } = default!;

        public string TmnCode { get; init; } = default!;

        public string HashSecret { get; init; } = default!;

        public string PaymentUrl { get; init; } = default!;

        public string ReturnUrl { get; init; } = default!;

        public string IpnUrl { get; init; } = default!;

        public string Locale { get; init; } = default!;

        public string Currency { get; init; } = default!;

        public string TimeZoneId { get; init; } = default!;
    }
}
