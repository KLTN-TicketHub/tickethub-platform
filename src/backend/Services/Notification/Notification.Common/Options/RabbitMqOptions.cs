namespace Notification.Common.Options
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = string.Empty;

        public ushort Port { get; set; } = 5672;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string VirtualHost { get; set; } = string.Empty;
    }
}
