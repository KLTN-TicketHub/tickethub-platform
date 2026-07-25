using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Features.Organizer.Staffs.Requests;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Identity.Application.Features.Organizer.Staffs.Commands.ActivateStaffAccount
{
    public class ActivateStaffAccountCommandHandler : IRequestHandler<ActivateStaffAccountCommand, Unit>
    {
        private readonly UserManager<User> _userManager;

        public ActivateStaffAccountCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(ActivateStaffAccountCommand command, CancellationToken cancellationToken)
        {
            await ActivateStaffAccountAsync(command.Request, cancellationToken);

            return Unit.Value;
        }

        private async Task ActivateStaffAccountAsync(ActivateStaffAccountRequest request, CancellationToken cancellationToken = default)
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

            string decodedToken = WebUtility.UrlDecode(request.Token);

            IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!confirmResult.Succeeded)
            {
                throw new BusinessRuleException(
                    $"Kích hoạt tài khoản thất bại: {string.Join(", ", confirmResult.Errors.Select(e => e.Description))}");
            }

            IdentityResult addPasswordResult = await _userManager.AddPasswordAsync(user, request.Password);
            if (!addPasswordResult.Succeeded)
            {
                user.EmailConfirmed = false;
                await _userManager.UpdateAsync(user);

                throw new BusinessRuleException(
                    $"Thiết lập mật khẩu thất bại: {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
            }

            user.SetUpdated(null);

            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new BusinessRuleException(
                    $"Cập nhật trạng thái kích hoạt thất bại: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
