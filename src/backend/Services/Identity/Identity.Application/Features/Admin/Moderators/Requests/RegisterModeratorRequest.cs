using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Admin.Moderators.Requests
{
    public class RegisterModeratorRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public IFormFile? Avatar { get; set; }
    }
}
