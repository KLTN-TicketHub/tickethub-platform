using AI.Common.Dtos.Suggestions;
using AI.Infrastructure.Interfaces.IServices;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/suggestions")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class SuggestionsController : ControllerBase
    {
        private readonly ISuggestionService _suggestionService;
        private readonly ICurrentUserService _currentUserService;

        public SuggestionsController(ISuggestionService suggestionService, ICurrentUserService currentUserService)
        {
            _suggestionService = suggestionService;
            _currentUserService = currentUserService;
        }

        [HttpGet("market-direction")]
        public async Task<IActionResult> GetMarketDirection([FromQuery] string range, CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            (bool isSuccess, string message, AiSuggestionResultDto? data) =
                await _suggestionService.GetMarketDirectionSuggestionsAsync(organizerId, NormalizeRange(range), cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse<AiSuggestionResultDto>(true, message, data));
        }

        [HttpGet("portfolio")]
        public async Task<IActionResult> GetPortfolio([FromQuery] string range, CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            (bool isSuccess, string message, AiSuggestionResultDto? data) =
                await _suggestionService.GetOrganizerPortfolioSuggestionsAsync(organizerId, NormalizeRange(range), cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse<AiSuggestionResultDto>(true, message, data));
        }

        [HttpGet("events/{eventId}")]
        public async Task<IActionResult> GetEventSuggestions(Guid eventId, [FromQuery] string range, CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            (bool isSuccess, string message, AiSuggestionResultDto? data) =
                await _suggestionService.GetEventSuggestionsAsync(organizerId, eventId, NormalizeRange(range), cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse<AiSuggestionResultDto>(true, message, data));
        }

        private static string NormalizeRange(string? range) => range == "7d" ? "7d" : "30d";
    }
}
