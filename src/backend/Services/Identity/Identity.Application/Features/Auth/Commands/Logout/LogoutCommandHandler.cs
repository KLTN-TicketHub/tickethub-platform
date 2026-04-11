using BuildingBlocks.Domain.Exceptions;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await LogoutAsync(request.RefreshToken, cancellationToken);
        }

        public async Task<bool> LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ValidatorException("Refresh token is required");

            RefreshToken? oldToken = await GetRefreshTokenAsync(refreshToken, cancellationToken);

            if (!oldToken.IsActive)
                throw new ValidatorException("Refresh token is not active or has expired");

            oldToken.Revoke();

            await _unitOfWork.RefreshTokenRepository.UpdateAsync(oldToken, cancellationToken);

            return true;
        }

        private async Task<RefreshToken> GetRefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                filter: r => r.Token == refreshToken,
                cancellation: cancellationToken) ?? throw new NotFoundException("Refresh token not found");
        }
    }
}
