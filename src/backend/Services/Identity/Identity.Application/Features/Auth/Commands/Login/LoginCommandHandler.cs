using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Request;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public LoginCommandHandler(ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<AuthDto> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<AuthDto> LoginAsync(
            LoginRequest request,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserName, cancellationToken);

            await CheckLockoutAsync(user);

            await CheckPasswordAsync(user, request.Password);

            await CheckEmailConfirmedAsync(user);

            AuthDto authDto = await CreateTokensAsync(user, deviceInfo, ipAddress);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authDto;
            throw new NotImplementedException();
        }

        private async Task<User> GetUserAsync(string userName, CancellationToken cancellationToken)
        {
            if (_currentUserService.IsAuthenticated)
                throw new ValidatorException("User is already authenticated");

            User user = await _userManager.FindByNameAsync(userName)
                ?? throw new NotFoundException($"User with UserName {userName} not found");

            return user;
        }

        private async Task CheckLockoutAsync(User user)
        {
            if (await _userManager.IsLockedOutAsync(user))
                throw new BusinessRuleException("User account is locked out!");
        }

        private async Task CheckPasswordAsync(User user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                throw new ValidatorException(nameof(LoginRequest.Password), $"Invalid password for user {user.UserName}");
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }
        }

        private async Task CheckEmailConfirmedAsync(User user)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new BusinessRuleException("Email not verified!");
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
    }
}
