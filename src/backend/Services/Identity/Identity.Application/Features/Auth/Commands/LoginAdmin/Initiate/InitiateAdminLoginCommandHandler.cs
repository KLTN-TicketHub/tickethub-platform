using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Features.Auth.Request;
using Identity.Domain.Entities;
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

        public InitiateAdminLoginCommandHandler(
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            ICacheService cacheService,
            IEventPublisher eventPublisher)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _cacheService = cacheService;
            _eventPublisher = eventPublisher;
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
                throw new NotFoundException($"User with UserName {request.UserName} not found");

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
                await _eventPublisher.PublishAsync(@event, cancellationToken);
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to publish email code event", ex);
            }

            return true;
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

        private async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
