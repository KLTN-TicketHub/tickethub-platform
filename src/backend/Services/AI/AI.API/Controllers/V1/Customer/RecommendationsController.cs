using AI.Infrastructure.Interfaces.IServices;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Controllers.V1.Customer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/recommendations")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Customer)]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ICurrentUserService _currentUserService;

        public RecommendationsController(IRecommendationService recommendationService, ICurrentUserService currentUserService)
        {
            _recommendationService = recommendationService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRecommendationsAsync([FromQuery] int topN, CancellationToken cancellationToken)
        {
            Guid userId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            List<RecommendationItemResult> recommendations = await _recommendationService.GetRecommendationsAsync(
                userId,
                topN > 0 ? topN : 10,
                cancellationToken);

            return Ok(new ApiResponse<List<RecommendationItemResult>>(true, "Lấy danh sách gợi ý thành công.", recommendations));
        }
    }
}
