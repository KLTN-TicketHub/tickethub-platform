using FluentValidation;
using Identity.Application.Features.Auth.Request;

namespace Identity.Application.Features.Auth.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(l => l.UserName)
                .NotEmpty().WithMessage($"{nameof(LoginRequest.UserName)} là bắt buộc")
                .MaximumLength(50).WithMessage($"{nameof(LoginRequest.UserName)} không được vượt quá 50 ký tự");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage($"{nameof(LoginRequest.Password)} là bắt buộc")
                .MinimumLength(6).WithMessage($"{nameof(LoginRequest.Password)} phải có ít nhất 6 ký tự")
                .MaximumLength(50).WithMessage("không được vượt quá 50 ký tự");
        }
    }
}
