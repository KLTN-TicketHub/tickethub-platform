using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Common.Interfaces.IExternalServices.IGoogleServices;
using Identity.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Identity.Common.Options;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Auth.Commands.LoginGoogle
{
    public class LoginWithGoogleCodeCommandHandler : IRequestHandler<LoginWithGoogleCodeCommand, AuthDto>
    {
        private const string GoogleProviderName = "Google";
        private const string CustomerRoleName = "Customer";

        private readonly IGoogleAuthService _googleAuthService;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly RoleManager<Role> _roleManager;
        private readonly AppSettings _appSettings;

        public LoginWithGoogleCodeCommandHandler(
            IGoogleAuthService googleAuthService,
            UserManager<User> userManager,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            RoleManager<Role> roleManager,
            IOptions<AppSettings> appSettings)
        {
            _googleAuthService = googleAuthService;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _roleManager = roleManager;
            _appSettings = appSettings.Value;
        }

        public Task<AuthDto> Handle(LoginWithGoogleCodeCommand request, CancellationToken cancellationToken)
        {
            return LoginWithGoogleAsync(request.Request.Code, request.Request.RedirectUri, cancellationToken);
        }

        private async Task<AuthDto> LoginWithGoogleAsync(string code, string redirectUri, CancellationToken cancellationToken = default)
        {
            if (_currentUserService.IsAuthenticated)
                throw new ValidatorException("Người dùng đã được xác thực");

            GoogleExchangeResult exchange = await _googleAuthService.ExchangeCodeAsync(code, redirectUri, cancellationToken);

            if (string.IsNullOrWhiteSpace(exchange.TokenPayload.Email))
                throw new BusinessRuleException("Tài khoản Google không có địa chỉ email.");

            if (!exchange.TokenPayload.EmailVerified)
                throw new BusinessRuleException("Email Google chưa được xác thực.");

            User user = await GetOrCreateCustomerUserAsync(exchange.TokenPayload, cancellationToken);

            IList<string> roles = await _userManager.GetRolesAsync(user);

            string accessToken = _jwtTokenService.GenerateJwtToken(user, roles);

            int refreshDays = _appSettings.JwtConfig!.RefreshTokenExpirationDays;

            RefreshToken newToken = new RefreshToken(
                user.Id,
                DateTime.UtcNow.AddDays(refreshDays),
                _currentUserService.DeviceInfo ?? string.Empty,
                _currentUserService.IpAddress ?? string.Empty);

            await _unitOfWork.RefreshTokenRepository.CreateAsync(newToken, cancellationToken);

            return new AuthDto
            {
                AccessToken = accessToken,
                RefreshToken = newToken.Token
            };
        }

        private async Task<User> GetOrCreateCustomerUserAsync(GoogleTokenPayloadDto payload, CancellationToken cancellation = default)
        {
            User? user = await FindLinkedGoogleUserAsync(payload.Subject);

            if (user != null)
            {
                await SyncGoogleProfileAsync(user, payload);
                return user;
            }

            user = await _userManager.FindByEmailAsync(payload.Email);

            if (user != null)
            {
                await EnsureUserIsCustomerAsync(user);
                await SyncGoogleProfileAsync(user, payload);
                await LinkGoogleLoginAsync(user, payload.Subject, cancellation);
                return user;
            }

            User newUser = new User
            {
                UserName = BuildUserName(payload),
                Email = payload.Email,
                FullName = string.IsNullOrWhiteSpace(payload.Name) ? payload.Email! : payload.Name,
                EmailConfirmed = true,
                ImageUrl = payload.Picture
            };

            IdentityResult createResult = await _userManager.CreateAsync(newUser);

            if (!createResult.Succeeded)
                throw new BusinessRuleException($"Không thể tạo người dùng: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            if (!await _roleManager.RoleExistsAsync(CustomerRoleName))
                throw new BusinessRuleException($"Vai trò {CustomerRoleName} không tồn tại.");

            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(newUser, CustomerRoleName);
            if (!addRoleResult.Succeeded)
                throw new BusinessRuleException($"Không thể gán vai trò: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");

            await LinkGoogleLoginAsync(newUser, payload.Subject, cancellation);

            return newUser;
        }

        private async Task<User?> FindLinkedGoogleUserAsync(string googleSubject)
        {
            User? user = await _userManager.FindByLoginAsync(GoogleProviderName, googleSubject);

            if (user != null)
                await EnsureUserIsCustomerAsync(user);
            return user;
        }

        private async Task EnsureUserIsCustomerAsync(User user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains(CustomerRoleName))
                throw new BusinessRuleException("Đăng nhập Google chỉ có sẵn cho tài khoản Khách hàng.");
        }

        private async Task LinkGoogleLoginAsync(User user, string googleSubject, CancellationToken cancellation = default)
        {
            IList<UserLoginInfo> logins = await _userManager.GetLoginsAsync(user);

            if (logins.Any(l => l.LoginProvider == GoogleProviderName && l.ProviderKey == googleSubject))
                return;

            IdentityResult loginResult = await _userManager.AddLoginAsync(
                user,
                new UserLoginInfo(GoogleProviderName, googleSubject, GoogleProviderName));

            if (!loginResult.Succeeded)
                throw new BusinessRuleException($"Không thể liên kết tài khoản Google: {string.Join(", ", loginResult.Errors.Select(e => e.Description))}");
        }

        private static string BuildUserName(GoogleTokenPayloadDto payload)
        {
            string local = payload.Email?.Split('@').FirstOrDefault() ?? "googleuser";
            string subpart = payload.Subject.Length > 8 ? payload.Subject.Substring(0, 8) : payload.Subject;
            return $"{local}-{subpart}".ToLowerInvariant();
        }

        private async Task SyncGoogleProfileAsync(User user, GoogleTokenPayloadDto payload)
        {
            bool hasChanges = false;

            string newFullName = string.IsNullOrWhiteSpace(payload.Name) ? payload.Email! : payload.Name;

            if (!string.IsNullOrWhiteSpace(newFullName) && user.FullName != newFullName)
            {
                user.FullName = newFullName;
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(payload.Picture) && user.ImageUrl != payload.Picture)
            {
                user.ImageUrl = payload.Picture;
                hasChanges = true;
            }

            if (hasChanges)
            {
                IdentityResult result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new BusinessRuleException(
                        $"Không thể cập nhật thông tin người dùng: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
