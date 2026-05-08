namespace Identity.Application.Features.Auth.Request
{
    public class ConfirmLoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
