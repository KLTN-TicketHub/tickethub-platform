using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Infrastructure.Auditing;
using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Features.Organizer.Staffs.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Organizer.Staffs.Commands.SetStaffLockStatus
{
    public class SetStaffLockStatusCommandHandler : IRequestHandler<SetStaffLockStatusCommand, StaffListItemDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public SetStaffLockStatusCommandHandler(
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

        public async Task<StaffListItemDto> Handle(SetStaffLockStatusCommand command, CancellationToken cancellationToken)
        {
            return await SetStaffLockStatusAsync(command.OrganizerId, command.StaffId, command.Request, cancellationToken);
        }

        private async Task<StaffListItemDto> SetStaffLockStatusAsync(
            Guid organizerId,
            Guid staffId,
            SetStaffLockStatusRequest request,
            CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByIdAsync(staffId.ToString())
                ?? throw new NotFoundException($"Không tìm thấy nhân viên với ID {staffId}.");

            if (user.CreatedBy != organizerId)
                throw new ForbiddenAccessException("Bạn không có quyền quản lý tài khoản nhân viên này.");

            if (!await _userManager.IsInRoleAsync(user, Roles.Staff))
                throw new BusinessRuleException("Tài khoản này không phải nhân viên soát vé.");

            await _userManager.SetLockoutEnabledAsync(user, true);

            IdentityResult lockResult = await _userManager.SetLockoutEndDateAsync(user, request.IsLocked ? DateTimeOffset.MaxValue : null);
            if (!lockResult.Succeeded)
                throw new BusinessRuleException(string.Join(", ", lockResult.Errors.Select(e => e.Description)));

            await CreateAuditLogAsync(request.IsLocked ? "Locked" : "Unlocked", user.Id, cancellationToken);

            StaffListItemDto result = _mapper.Map<StaffListItemDto>(user);
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
                Source = "OrganizerStaffManagement"
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
