namespace Identity.Common.Options
{
    public class GoogleAuthSettings
    {
        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;

        public string[]? Scopes { get; set; } = new string[]
        {
            "openid",
            "email",
            "profile"
        };
    }
}
