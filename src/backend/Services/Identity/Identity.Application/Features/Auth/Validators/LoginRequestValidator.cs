using FluentValidation;
using Identity.Application.Features.Auth.Requests;

namespace Identity.Application.Features.Auth.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(l => l.UserName)
                .NotEmpty().WithMessage($"{nameof(LoginRequest.UserName)} là bắt buộc")
                .MinimumLength(5).WithMessage($"{nameof(LoginRequest.UserName)} phải có ít nhất 5 ký tự")
                .MaximumLength(50).WithMessage($"{nameof(LoginRequest.UserName)} không được vượt quá 50 ký tự");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu là bắt buộc")
                .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
                .MaximumLength(50).WithMessage("Mật khẩu không được vượt quá 50 ký tự")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ cái viết hoa")
                .Matches(@"[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ cái viết thường")
                .Matches(@"[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số")
                .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-\+\=\<\>\?]").WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt");
        }
    }
}
