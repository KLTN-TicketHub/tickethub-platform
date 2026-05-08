using BuildingBlocks.Contracts.Options;

namespace Identity.Common.Options
{
    public class AppSettings
    {
        public JwtConfig? JwtConfig { get; set; }

        public RateLimitConfig? RateLimit { get; set; }
    }
}
