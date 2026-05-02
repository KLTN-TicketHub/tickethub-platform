using Identity.Application.Common.DTOs.Auth;

namespace Identity.Application.Common.Interfaces.IExternalServices.IGoogleServices
{
    public interface IGoogleTokenVerifierService
    {
        Task<GoogleTokenPayloadDto> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
    }
}
