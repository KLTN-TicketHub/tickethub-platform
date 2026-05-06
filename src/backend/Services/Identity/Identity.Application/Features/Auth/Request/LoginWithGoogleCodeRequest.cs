namespace Identity.Application.Features.Auth.Request
{
    public class LoginWithGoogleCodeRequest
    {
        public string Code { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;
    }
}
