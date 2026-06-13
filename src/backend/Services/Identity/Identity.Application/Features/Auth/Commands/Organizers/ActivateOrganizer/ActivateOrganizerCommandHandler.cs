using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Features.Auth.Requests;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Auth.Commands.Organizers.ActivateOrganizer
{
    public class ActivateOrganizerCommandHandler : IRequestHandler<ActivateOrganizerCommand, Unit>
    {
        private readonly UserManager<User> _userManager;

        public ActivateOrganizerCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(ActivateOrganizerCommand command, CancellationToken cancellationToken)
        {
            return await ActivateOrganizerAsync(command.Request, cancellationToken);
        }

        private async Task<Unit> ActivateOrganizerAsync(ActivateOrganizerAccountRequest request, CancellationToken cancellationToken = default)
        {
            User? user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException($"Không tìm thấy tài khoản với ID '{request.UserId}'.");
            }

            if (user.EmailConfirmed && !string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new BusinessRuleException("Tài khoản này đã được kích hoạt trước đó.");
            }

            IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!confirmResult.Succeeded)
            {
                throw new BusinessRuleException(
                    $"Kích hoạt tài khoản thất bại: {string.Join(", ", confirmResult.Errors.Select(e => e.Description))}");
            }

            user.SetUpdated(null);

            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new BusinessRuleException(
                    $"Cập nhật trạng thái kích hoạt thất bại: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
            }

            return Unit.Value;
        }
    }
}
