using AI.Infrastructure.Interfaces.IServices;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/recommendations")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Admin)]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpPost("train")]
        public async Task<IActionResult> TrainAsync(CancellationToken cancellationToken)
        {
            await _recommendationService.SyncAndTrainAsync(cancellationToken);

            return Ok(new ApiResponse(true, "Đã đồng bộ dữ liệu và train lại mô hình recommendation."));
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatusAsync(CancellationToken cancellationToken)
        {
            TrainingStatusResult status = await _recommendationService.GetTrainingStatusAsync(cancellationToken);

            return Ok(new ApiResponse<TrainingStatusResult>(true, "Lấy trạng thái mô hình recommendation thành công.", status));
        }
    }
}
