using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Auth.Commands.Organizers.RegisterOrganizer
{
    public class RegisterOrganizerCommandHandler : IRequestHandler<RegisterOrganizerCommand, OrganizerDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterOrganizerCommandHandler(
            UserManager<User> userManager,
            IEventPublisher eventPublisher,
            IFileService fileService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _eventPublisher = eventPublisher;
            _fileService = fileService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizerDto> Handle(RegisterOrganizerCommand command, CancellationToken cancellationToken)
        {
            return await RegisterOrganizerAysnc(command.Request, cancellationToken);
        }

        private async Task<OrganizerDto> RegisterOrganizerAysnc(RegisterOrganizerRequest request, CancellationToken cancellationToken = default)
        {
            await EnsureEmailNotExistsAsync(request.Email);
            await EnsureUserNameNotExistsAsync(request.UserName);

            User user = new User
            {
                UserName = request.UserName,
                FullName = request.OrganizerName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            IdentityResult createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
                throw new BusinessRuleException(
                    $"Tạo tài khoản Organizer thất bại: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            string activationToken =
               await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _userManager.AddToRoleAsync(user, Roles.Organizer);

            PublishActivationEventAsync(user, activationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            OrganizerDto result = _mapper.Map<OrganizerDto>(user);

            return result;
        }

        private async Task EnsureEmailNotExistsAsync(string email)
        {
            User? existing = await _userManager.FindByEmailAsync(email);

            if (existing is not null)
                throw new BusinessRuleException($"Email '{email}' đã được sử dụng bởi tài khoản khác.");
        }

        private async Task EnsureUserNameNotExistsAsync(string userName)
        {
            User? existing = await _userManager.FindByNameAsync(userName);

            if (existing is not null)
                throw new BusinessRuleException($"Username '{userName}' đã tồn tại.");
        }

        private void PublishActivationEventAsync(
            User user,
            string activationToken)
        {
            OrganizerRegisteredEvent @event = new OrganizerRegisteredEvent
            {
                UserId = user.Id.ToString(),
                Email = user.Email!,
                FullName = user.FullName,
                UserName = user.UserName!,
                ActivationToken = activationToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };

            _eventPublisher.Publish(@event);
        }
    }
}
