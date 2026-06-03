using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StackExchange.Redis;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VenuesController : ControllerBase
    {
        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public IActionResult CreateVenueAsync()
        {
            throw new NotImplementedException();
        }
    }
}
