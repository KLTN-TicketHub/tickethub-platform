using FluentValidation;
using Identity.Application.Features.Admin.Moderators.Requests;

namespace Identity.Application.Features.Admin.Moderators.Validators
{
    public class RegisterModeratorRequestValidator : AbstractValidator<RegisterModeratorRequest>
    {
        public RegisterModeratorRequestValidator()
        {
            RuleFor(r => r.FullName)
                .NotEmpty().WithMessage($"{nameof(RegisterModeratorRequest.FullName)} là bắt buộc")
                .MaximumLength(100).WithMessage($"{nameof(RegisterModeratorRequest.FullName)} không được vượt quá 100 ký tự");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage($"{nameof(RegisterModeratorRequest.Email)} là bắt buộc")
                .EmailAddress().WithMessage($"{nameof(RegisterModeratorRequest.Email)} không đúng định dạng email")
                .MaximumLength(150).WithMessage($"{nameof(RegisterModeratorRequest.Email)} không được vượt quá 150 ký tự");

            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage($"{nameof(RegisterModeratorRequest.PhoneNumber)} là bắt buộc")
                .Matches(@"^(0|\+84)[3-9]\d{8}$").WithMessage($"{nameof(RegisterModeratorRequest.PhoneNumber)} không đúng định dạng số điện thoại Việt Nam")
                .MaximumLength(15).WithMessage($"{nameof(RegisterModeratorRequest.PhoneNumber)} không được vượt quá 15 ký tự");

            RuleFor(r => r.Avatar)
                .Must(file => file == null || file.Length > 0)
                .WithMessage($"{nameof(RegisterModeratorRequest.Avatar)} không được là file rỗng");
        }
    }
}
