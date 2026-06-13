namespace Identity.Application.Features.Auth.Requests
{
    public class RegisterOrganizerRequest
    {
        public string UserName { get; set; }

        public string OrganizerName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
