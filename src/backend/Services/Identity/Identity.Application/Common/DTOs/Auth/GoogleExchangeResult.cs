namespace Identity.Application.Common.DTOs.Auth
{
    public class GoogleExchangeResult
    {
        public string AccessToken { get; set; } = string.Empty;

        public string IdToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }

        public GoogleTokenPayloadDto TokenPayload { get; set; } = new GoogleTokenPayloadDto();
    }
}
