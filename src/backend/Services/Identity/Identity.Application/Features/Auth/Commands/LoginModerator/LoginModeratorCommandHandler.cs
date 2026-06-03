using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Identity.Application.Features.Auth.Requests;
using Identity.Common.Options;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Auth.Commands.LoginModerator
{
    public class LoginModeratorCommandHandler : IRequestHandler<LoginModeratorCommand, AuthDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettings _appSettings;

        public LoginModeratorCommandHandler(
            ICurrentUserService currentUserService,
             UserManager<User> userManager,
             IUnitOfWork unitOfWork,
             IJwtTokenService jwtTokenService,
             IOptions<AppSettings> appSettings)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthDto> Handle(
            LoginModeratorCommand request,
            CancellationToken cancellationToken)
        {
            return await LoginAsync(
                request.Request,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                cancellationToken);
        }

        private async Task<AuthDto> LoginAsync(
            LoginRequest request,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserName, cancellationToken);

            IList<string> roles = await GetUserRolesAsync(user);

            if (!roles.Contains("Moderator"))
                throw new NotFoundException($"Không tìm thấy người dùng với tên {request.UserName}");

            await CheckLockoutAsync(user);

            await CheckPasswordAsync(user, request.Password);

            await CheckEmailConfirmedAsync(user);

            AuthDto authDto = await CreateTokensAsync(user, deviceInfo, ipAddress, roles);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authDto;
        }

        private async Task<User> GetUserAsync(string userName, CancellationToken cancellationToken)
        {
            if (_currentUserService.IsAuthenticated)
                throw new ValidatorException("Người dùng đã được xác thực");

            User user = await _userManager.FindByNameAsync(userName)
                ?? throw new NotFoundException($"Không tìm thấy người dùng với tên {userName}");

            return user;
        }

        private async Task CheckLockoutAsync(User user)
        {
            if (await _userManager.IsLockedOutAsync(user))
                throw new BusinessRuleException("Tài khoản người dùng bị khóa!");
        }

        private async Task CheckPasswordAsync(User user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                throw new ValidatorException(nameof(LoginRequest.Password), $"Mật khẩu không hợp lệ cho người dùng {user.UserName}");
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }
        }

        private async Task CheckEmailConfirmedAsync(User user)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new BusinessRuleException("Email chưa được xác thực!");
        }

        private async Task<AuthDto> CreateTokensAsync(
            User user, string? deviceInfo, string? ipAddress, IList<string> roles)
        {
            string accessToken = _jwtTokenService.GenerateJwtToken(user, roles);

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
