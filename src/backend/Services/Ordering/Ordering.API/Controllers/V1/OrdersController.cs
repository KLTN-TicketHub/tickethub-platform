using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Common.Dtos;
using Ordering.Infrastructure.Interfaces.IServices;
using System.Security.Claims;
using BuildingBlocks.Contracts.Models.Responses;

namespace Ordering.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            (bool isSuccess, Guid orderId, string message) = await _orderService.CheckoutAsync(request, Guid.Parse(userIdClaim!));

            if (!isSuccess)
            {
                return BadRequest(new ApiResponse(false, message));
            }

            return Ok(new ApiResponse<Guid>(true, message, orderId));
        }
    }
}
