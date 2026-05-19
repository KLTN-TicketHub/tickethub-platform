namespace Identity.Common.Options
{
    public class JwtConfig
    {
        public string? PrivateKeyPath { get; set; }
        public string? PublicKeyPath { get; set; }
        public string? KeyId { get; set; }
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public int TokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
}
