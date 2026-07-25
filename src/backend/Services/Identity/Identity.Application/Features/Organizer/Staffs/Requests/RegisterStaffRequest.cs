namespace Identity.Application.Features.Organizer.Staffs.Requests
{
    public class RegisterStaffRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
