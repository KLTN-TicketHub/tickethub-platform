using FluentValidation;
using Identity.Application.Features.Organizer.Staffs.Requests;

namespace Identity.Application.Features.Organizer.Staffs.Validators
{
    public class RegisterStaffRequestValidator : AbstractValidator<RegisterStaffRequest>
    {
        public RegisterStaffRequestValidator()
        {
            RuleFor(r => r.FullName)
                .NotEmpty().WithMessage($"{nameof(RegisterStaffRequest.FullName)} là bắt buộc")
                .MaximumLength(100).WithMessage($"{nameof(RegisterStaffRequest.FullName)} không được vượt quá 100 ký tự");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage($"{nameof(RegisterStaffRequest.Email)} là bắt buộc")
                .EmailAddress().WithMessage($"{nameof(RegisterStaffRequest.Email)} không đúng định dạng email")
                .MaximumLength(150).WithMessage($"{nameof(RegisterStaffRequest.Email)} không được vượt quá 150 ký tự");

            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage($"{nameof(RegisterStaffRequest.PhoneNumber)} là bắt buộc")
                .Matches(@"^(0|\+84)[3-9]\d{8}$").WithMessage($"{nameof(RegisterStaffRequest.PhoneNumber)} không đúng định dạng số điện thoại Việt Nam")
                .MaximumLength(15).WithMessage($"{nameof(RegisterStaffRequest.PhoneNumber)} không được vượt quá 15 ký tự");
        }
    }
}
