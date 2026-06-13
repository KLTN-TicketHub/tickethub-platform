namespace Identity.Application.Features.Auth.Requests
{
    public class ActivateOrganizerAccountRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
