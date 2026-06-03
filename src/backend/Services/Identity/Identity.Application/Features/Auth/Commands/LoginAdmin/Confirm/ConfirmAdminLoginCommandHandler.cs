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

namespace Identity.Application.Features.Auth.Commands.LoginAdmin.Confirm
{
    public class ConfirmAdminLoginCommandHandler : IRequestHandler<ConfirmAdminLoginCommand, AuthDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettings _appSettings;
        private readonly ICacheService _cacheService;

        public ConfirmAdminLoginCommandHandler(
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IJwtTokenService jwtTokenService,
            IOptions<AppSettings> appSettings,
            ICacheService cacheService)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _appSettings = appSettings.Value;
            _cacheService = cacheService;
        }

        public async Task<AuthDto> Handle(
            ConfirmAdminLoginCommand request,
            CancellationToken cancellationToken)
        {
            return await ConfirmLoginAsync(
                request.Request,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                cancellationToken);
        }

        private async Task<AuthDto> ConfirmLoginAsync(
            ConfirmLoginRequest request,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Code))
                throw new ValidatorException("Tên người dùng và Mã là bắt buộc");

            string key = $"auth:verify:admin:{request.UserName}:login";
            string? storedCode = await _cacheService.GetAsync<string>(key, cancellationToken);

            if (string.IsNullOrEmpty(storedCode))
                throw new ValidatorException("Mã xác thực đã hết hạn hoặc không hợp lệ");

            if (storedCode != request.Code)
                throw new ValidatorException("Mã xác thực không chính xác");

            await _cacheService.RemoveAsync(key, cancellationToken);

            User user = await _userManager.FindByNameAsync(request.UserName)
                ?? throw new NotFoundException($"Không tìm thấy người dùng với tên {request.UserName}");

            IList<string> roles = await GetUserRolesAsync(user);

            if (!roles.Contains("Admin"))
                throw new NotFoundException($"Không tìm thấy người dùng với tên {request.UserName}");

            AuthDto authDto = await CreateTokensAsync(user, deviceInfo, ipAddress, roles);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authDto;
        }

        private async Task<AuthDto> CreateTokensAsync(
            User user, string? deviceInfo, string? ipAddress, IList<string> roles)
        {
            string accessToken = _jwtTokenService.GenerateJwtToken(user, roles);

            RefreshToken newToken = new RefreshToken(
                user.Id,
                DateTime.UtcNow.AddDays(_appSettings.JwtConfig!.RefreshTokenExpirationDays),
                deviceInfo!,
                ipAddress!);

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
