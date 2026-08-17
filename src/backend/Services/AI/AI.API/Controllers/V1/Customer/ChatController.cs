using AI.Common.Dtos.Chat;
using AI.Infrastructure.Interfaces.IServices;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Controllers.V1.Customer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/chat")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ICurrentUserService _currentUserService;

        public ChatController(IChatService chatService, ICurrentUserService currentUserService)
        {
            _chatService = chatService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request, CancellationToken cancellationToken)
        {
            Guid userId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            (bool isSuccess, string message, ChatResponseDto? data) =
                await _chatService.SendMessageAsync(userId, request.SessionId, request.Message, cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse<ChatResponseDto>(true, message, data));
        }
    }
}
