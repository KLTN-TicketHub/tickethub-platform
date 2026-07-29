using FluentValidation;
using Identity.Application.Features.Admin.Users.Requests;

namespace Identity.Application.Features.Admin.Users.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(r => r.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{nameof(UpdateUserRequest.FullName)} là bắt buộc")
                .MaximumLength(100).WithMessage($"{nameof(UpdateUserRequest.FullName)} không được vượt quá 100 ký tự");

            RuleFor(r => r.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .Matches(@"^(0|\+84)[3-9]\d{8}$").WithMessage($"{nameof(UpdateUserRequest.PhoneNumber)} không đúng định dạng số điện thoại Việt Nam")
                .MaximumLength(15).WithMessage($"{nameof(UpdateUserRequest.PhoneNumber)} không được vượt quá 15 ký tự")
                .When(r => !string.IsNullOrWhiteSpace(r.PhoneNumber));
        }
    }
}
