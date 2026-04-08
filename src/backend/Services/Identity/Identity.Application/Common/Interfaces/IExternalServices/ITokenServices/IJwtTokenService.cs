using Identity.Domain.Entities;

namespace Identity.Application.Common.Interfaces.IExternalServices.ITokenServices
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user, IList<string> roles);
    }
}
