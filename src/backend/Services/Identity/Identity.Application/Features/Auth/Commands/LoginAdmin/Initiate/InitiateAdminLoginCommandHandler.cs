using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Features.Auth.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Identity.Application.Features.Auth.Commands.LoginAdmin.Initiate
{
    public class InitiateAdminLoginCommandHandler : IRequestHandler<InitiateAdminLoginCommand, Unit>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly ICacheService _cacheService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUnitOfWork _unitOfWork;

        public InitiateAdminLoginCommandHandler(
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            ICacheService cacheService,
            IEventPublisher eventPublisher,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _cacheService = cacheService;
            _eventPublisher = eventPublisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            InitiateAdminLoginCommand request,
            CancellationToken cancellationToken)
        {
            await InitiateLoginAsync(
                request.Request,
                cancellationToken);

            return Unit.Value;
        }

        private async Task<bool> InitiateLoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserName, cancellationToken);

            IList<string> roles = await GetUserRolesAsync(user);

            if (!roles.Contains("Admin"))
                throw new NotFoundException($"Không tìm thấy người dùng với tên {request.UserName}");

            await CheckLockoutAsync(user);

            await CheckPasswordAsync(user, request.Password);

            await CheckEmailConfirmedAsync(user);

            int rng = RandomNumberGenerator.GetInt32(100000, 999999);
            string code = rng.ToString();

            string key = $"auth:verify:admin:{request.UserName}:login";

            TimeSpan ttl = TimeSpan.FromMinutes(5);

            await _cacheService.SetAsync(key, code, ttl, cancellationToken);

            SendEmailCodeEvent @event = new SendEmailCodeEvent
            {
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            try
            {
                _eventPublisher.Publish(@event);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể xuất bản sự kiện mã email", ex);
            }

            return true;
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

        private async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
