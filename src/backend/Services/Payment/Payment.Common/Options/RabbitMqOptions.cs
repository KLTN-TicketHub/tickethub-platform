namespace Payment.Common.Options
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = string.Empty;

        public ushort Port { get; set; } = 5672;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string VirtualHost { get; set; } = string.Empty;
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
