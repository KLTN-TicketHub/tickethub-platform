using FluentValidation;
using Identity.Application.Features.Admin.Moderators.Requests;
using Identity.Application.Features.Auth.Requests;

namespace Identity.Application.Features.Auth.Validators
{
    public class RegisterOrganizerRequestValidator : AbstractValidator<RegisterOrganizerRequest>
    {
        public RegisterOrganizerRequestValidator()
        {
            RuleFor(r => r.UserName)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự")
                .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự");

            RuleFor(r => r.OrganizerName)
                .NotEmpty().WithMessage("Tên tổ chức không được để trống")
                .MinimumLength(3).WithMessage("Tên tổ chức phải có ít nhất 3 ký tự")
                .MaximumLength(100).WithMessage("Tên tổ chức không được vượt quá 100 ký tự");

            RuleFor(r => r.Email)
                 .NotEmpty().WithMessage($"{nameof(RegisterModeratorRequest.Email)} là bắt buộc")
                 .EmailAddress().WithMessage($"{nameof(RegisterModeratorRequest.Email)} không đúng định dạng email")
                 .MaximumLength(150).WithMessage($"{nameof(RegisterModeratorRequest.Email)} không được vượt quá 150 ký tự");

            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage($"{nameof(RegisterModeratorRequest.PhoneNumber)} là bắt buộc")
                .Matches(@"^(0|\+84)[3-9]\d{8}$").WithMessage($"{nameof(RegisterModeratorRequest.PhoneNumber)} không đúng định dạng số điện thoại Việt Nam")
                .MaximumLength(15).WithMessage($"{nameof(RegisterModeratorRequest.PhoneNumber)} không được vượt quá 15 ký tự");

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
                .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp với mật khẩu");
        }
    }
}
