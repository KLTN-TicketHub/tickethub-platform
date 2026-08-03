using BuildingBlocks.Contracts.Constants;
using FluentValidation;
using Notification.Common.Dtos.Notifications;

namespace Notification.API.Validators
{
    public class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
    {
        private static readonly string[] AllowedRoles =
        {
            Roles.Admin,
            Roles.Customer,
            Roles.Moderator,
            Roles.Organizer,
            Roles.Staff
        };

        public SendNotificationRequestValidator()
        {
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Tiêu đề thông báo không được để trống.")
                .MaximumLength(200).WithMessage("Tiêu đề thông báo không được vượt quá 200 ký tự.");

            RuleFor(x => x.Message)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nội dung thông báo không được để trống.")
                .MaximumLength(1000).WithMessage("Nội dung thông báo không được vượt quá 1000 ký tự.");

            RuleFor(x => x.LinkUrl)
                .MaximumLength(500).WithMessage("Đường dẫn đính kèm không được vượt quá 500 ký tự.");

            RuleFor(x => x.TargetRole)
                .Must(role => AllowedRoles.Contains(role))
                .When(x => !string.IsNullOrEmpty(x.TargetRole))
                .WithMessage($"Vai trò nhận thông báo phải là một trong các giá trị: {string.Join(", ", AllowedRoles)}.");

            RuleFor(x => x.TargetRole)
                .Empty()
                .When(x => x.RecipientUserId.HasValue)
                .WithMessage("Không thể vừa chọn người nhận cụ thể vừa chọn vai trò nhận thông báo.");
        }
    }
}
