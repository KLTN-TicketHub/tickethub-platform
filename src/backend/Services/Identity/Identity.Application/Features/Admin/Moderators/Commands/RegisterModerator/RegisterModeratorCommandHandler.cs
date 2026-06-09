using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Common.Interfaces.IBackgroundJobs;
using Identity.Application.Features.Admin.Moderators.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Identity.Application.Features.Admin.Moderators.Commands.RegisterModerator
{
    public class RegisterModeratorCommandHandler : IRequestHandler<RegisterModeratorCommand, ModeratorDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobService _backgroundJobService;

        public RegisterModeratorCommandHandler(
            UserManager<User> userManager,
            IEventPublisher eventPublisher,
            IFileService fileService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IBackgroundJobService backgroundJobService)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
            _fileService = fileService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<ModeratorDto> Handle(
            RegisterModeratorCommand request,
            CancellationToken cancellationToken)
        {
            return await RegisterAsync(request.Request, cancellationToken);
        }

        private async Task<ModeratorDto> RegisterAsync(
            RegisterModeratorRequest request,
            CancellationToken cancellationToken = default)
        {
            await EnsureEmailNotExistsAsync(request.Email);

            string? imageUrl = await UploadAvatarAsync(request, cancellationToken);

            string userName = GenerateUserName(request.Email);

            User user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = userName,
                PhoneNumber = request.PhoneNumber,
                ImageUrl = imageUrl,
            };

            IdentityResult createResult = await _userManager.CreateAsync(user);

            string activationToken =
               await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (!createResult.Succeeded)
                throw new BusinessRuleException(
                    $"Tạo tài khoản Moderator thất bại: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, Roles.Moderator);

            PublishActivationEventAsync(user, activationToken);
            _backgroundJobService.ScheduleDeletePendingUser(user, TimeSpan.FromMinutes(15));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            ModeratorDto result = _mapper.Map<ModeratorDto>(user);
            result.ImageUrl = user.ImageUrl != null ? _fileService.GetAbsoluteUrl(user.ImageUrl) : null;

            return result;
        }

        private async Task EnsureEmailNotExistsAsync(string email)
        {
            User? existing = await _userManager.FindByEmailAsync(email);

            if (existing is not null)
                throw new BusinessRuleException($"Email '{email}' đã được sử dụng bởi tài khoản khác.");
        }

        private static string GenerateUserName(string email)
        {
            string prefix = email.Split('@')[0].ToLower().Replace(".", "_");
            int suffix = RandomNumberGenerator.GetInt32(1000, 9999);
            return $"{prefix}_{suffix}";
        }

        private async Task<string?> UploadAvatarAsync(
            RegisterModeratorRequest request,
            CancellationToken cancellationToken)
        {
            if (request.Avatar is null)
                return null;

            if (!_fileService.IsValidFile(request.Avatar))
                throw new ValidatorException(nameof(request.Avatar), "File ảnh không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .webp.");

            return await _fileService.SaveFileAsync(request.Avatar, "moderators", cancellationToken);
        }

        private void PublishActivationEventAsync(
            User user,
            string activationToken)
        {
            ModeratorRegisteredEvent @event = new ModeratorRegisteredEvent
            {
                UserId = user.Id.ToString(),
                Email = user.Email!,
                FullName = user.FullName,
                UserName = user.UserName!,
                ActivationToken = activationToken,
                ExpiredAt = DateTime.UtcNow.AddMinutes(15)
            };

            _eventPublisher.Publish(@event);
        }
    }
}
