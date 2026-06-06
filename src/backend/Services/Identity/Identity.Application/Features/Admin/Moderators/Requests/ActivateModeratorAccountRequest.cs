namespace Identity.Application.Features.Admin.Moderators.Requests
{
    public class ActivateModeratorAccountRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
