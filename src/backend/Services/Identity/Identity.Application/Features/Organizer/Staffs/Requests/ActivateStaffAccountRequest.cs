namespace Identity.Application.Features.Organizer.Staffs.Requests
{
    public class ActivateStaffAccountRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
