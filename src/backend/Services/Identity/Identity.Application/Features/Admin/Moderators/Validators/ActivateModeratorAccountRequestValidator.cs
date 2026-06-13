using FluentValidation;
using Identity.Application.Features.Admin.Moderators.Requests;

namespace Identity.Application.Features.Admin.Moderators.Validators
{
    public class ActivateModeratorAccountRequestValidator : AbstractValidator<ActivateModeratorAccountRequest>
    {
        public ActivateModeratorAccountRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId là bắt buộc");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token kích hoạt là bắt buộc");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu là bắt buộc")
                .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
                .MaximumLength(50).WithMessage("Mật khẩu không được vượt quá 50 ký tự")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ cái viết hoa")
                .Matches(@"[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ cái viết thường")
                .Matches(@"[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số")
                .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-\+\=\<\>\?]").WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu là bắt buộc")
                .Equal(x => x.Password).WithMessage("Mật khẩu và xác nhận mật khẩu không khớp");
        }
    }
}
