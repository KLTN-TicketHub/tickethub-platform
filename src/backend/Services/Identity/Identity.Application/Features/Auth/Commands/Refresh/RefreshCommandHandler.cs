using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Identity.Common.Options;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettings _appSettings;

        public RefreshCommandHandler(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IJwtTokenService jwtTokenService,
            IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            return await RefreshAsync(request.RefreshToken, null, null, cancellationToken);
        }

        public async Task<AuthDto> RefreshAsync(
            string? refreshToken,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                RefreshToken? oldToken = await GetRefreshTokenAsync(refreshToken, cancellationToken);

                if (!oldToken.IsActive)
                    throw new ValidatorException("Refresh token không hoạt động hoặc đã hết hạn");

                User? user = await _userManager.FindByIdAsync(oldToken.UserId.ToString())
                    ?? throw new NotFoundException("Không tìm thấy người dùng");

                oldToken.Revoke();

                AuthDto result = await CreateTokensAsync(user, deviceInfo, ipAddress);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return result;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private async Task<RefreshToken> GetRefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                filter: r => r.Token == refreshToken,
                cancellation: cancellationToken) ?? throw new NotFoundException("Không tìm thấy refresh token");
        }

        private async Task<AuthDto> CreateTokensAsync(
            User user, string? deviceInfo, string? ipAddress)
        {
            string accessToken = _jwtTokenService.GenerateJwtToken(user, await GetUserRolesAsync(user));

            RefreshToken newToken = new RefreshToken(
                user.Id,
                DateTime.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays),
                deviceInfo,
                ipAddress);

            _unitOfWork.RefreshTokenRepository.AddEntity(newToken);

            return new AuthDto
            {
                AccessToken = accessToken,
                RefreshToken = newToken.Token
            };
        }

        private async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
