using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Common.Interfaces.IBackgroundJobs;
using Identity.Application.Features.Organizer.Staffs.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Identity.Application.Features.Organizer.Staffs.Commands.RegisterStaff
{
    public class RegisterStaffCommandHandler : IRequestHandler<RegisterStaffCommand, StaffDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobService _backgroundJobService;

        public RegisterStaffCommandHandler(
            UserManager<User> userManager,
            IEventPublisher eventPublisher,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IBackgroundJobService backgroundJobService)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<StaffDto> Handle(
            RegisterStaffCommand command,
            CancellationToken cancellationToken)
        {
            return await RegisterAsync(command.OrganizerId, command.Request, cancellationToken);
        }

        private async Task<StaffDto> RegisterAsync(
            Guid organizerId,
            RegisterStaffRequest request,
            CancellationToken cancellationToken = default)
        {
            await EnsureEmailNotExistsAsync(request.Email);

            string userName = GenerateUserName(request.Email);

            User user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = userName,
                PhoneNumber = request.PhoneNumber,
            };

            user.SetCreated(organizerId);

            IdentityResult createResult = await _userManager.CreateAsync(user);

            string activationToken =
               await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (!createResult.Succeeded)
                throw new BusinessRuleException(
                    $"Tạo tài khoản Staff thất bại: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, Roles.Staff);

            await PublishActivationEventAsync(user, activationToken);
            _backgroundJobService.ScheduleDeletePendingUser(user, TimeSpan.FromMinutes(15));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<StaffDto>(user);
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

        private async Task PublishActivationEventAsync(
            User user,
            string activationToken)
        {
            StaffRegisteredEvent @event = new StaffRegisteredEvent
            {
                UserId = user.Id.ToString(),
                Email = user.Email!,
                FullName = user.FullName,
                UserName = user.UserName!,
                ActivationToken = activationToken,
                ExpiredAt = DateTime.UtcNow.AddMinutes(15)
            };

            await _eventPublisher.PublishAsync(@event);
        }
    }
}
