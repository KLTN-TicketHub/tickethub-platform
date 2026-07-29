using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Infrastructure.Auditing;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Users.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Admin.Users.Commands.SetUserLockStatus
{
    public class SetUserLockStatusCommandHandler : IRequestHandler<SetUserLockStatusCommand, UserDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public SetUserLockStatusCommandHandler(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UserDetailDto> Handle(SetUserLockStatusCommand command, CancellationToken cancellationToken)
        {
            return await SetUserLockStatusAsync(command.Id, command.Request, cancellationToken);
        }

        private async Task<UserDetailDto> SetUserLockStatusAsync(Guid id, SetUserLockStatusRequest request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == id)
                throw new BusinessRuleException("Không thể tự khoá tài khoản của chính mình.");

            User user = await _userManager.FindByIdAsync(id.ToString())
                ?? throw new NotFoundException($"Không tìm thấy người dùng với ID {id}.");

            await _userManager.SetLockoutEnabledAsync(user, true);

            IdentityResult lockResult = await _userManager.SetLockoutEndDateAsync(user, request.IsLocked ? DateTimeOffset.MaxValue : null);
            if (!lockResult.Succeeded)
                throw new BusinessRuleException(string.Join(", ", lockResult.Errors.Select(e => e.Description)));

            await CreateAuditLogAsync(request.IsLocked ? "Locked" : "Unlocked", user.Id, cancellationToken);

            UserDetailDto result = _mapper.Map<UserDetailDto>(user);
            result.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            result.IsLocked = await _userManager.IsLockedOutAsync(user);

            return result;
        }

        private async Task CreateAuditLogAsync(string action, Guid entityId, CancellationToken cancellationToken)
        {
            await _unitOfWork.AuditLogRepository.CreateAsync(new AuditLog
            {
                UserId = _currentUserService.UserId,
                UserName = _currentUserService.UserName,
                Action = action,
                EntityType = nameof(User),
                EntityId = entityId,
                IPAddress = _currentUserService.IpAddress,
                UserAgent = _currentUserService.DeviceInfo,
                Source = "AdminUserManagement"
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
