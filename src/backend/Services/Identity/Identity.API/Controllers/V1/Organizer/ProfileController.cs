using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Identity.Application.Features.Organizer.Profile.Commands.UpdateOrganizerAvatar;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/profile")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class ProfileController : ControllerBase
    {
        private readonly ISender _sender;

        public ProfileController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatarAsync(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            string result = await _sender.Send(new UpdateOrganizerAvatarCommand(file), cancellationToken);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Cập nhật ảnh đại diện thành công.",
                Data = result
            });
        }
    }
}
