using Identity.Application.Common.DTOs.Auth;

namespace Identity.Application.Common.Interfaces.IExternalServices.IGoogleServices
{
    public interface IGoogleAuthService
    {
        Task<GoogleExchangeResult> ExchangeCodeAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
    }
}
