using Catalog.Application.Features.Events.Requests;
using FluentValidation;

namespace Catalog.Application.Features.Events.Validators
{
    public class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationRequestValidator()
        {
            RuleFor(x => x.VenueName)
                .NotEmpty().WithMessage("Tên địa điểm không được để trống.")
                .MaximumLength(100).WithMessage("Tên địa điểm không được vượt quá 100 ký tự.");

            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Địa chỉ không được để trống.")
                .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự.");

            RuleFor(x => x.Ward)
                .NotEmpty().WithMessage("Phường/Xã không được để trống.")
                .MaximumLength(100).WithMessage("Phường/Xã không được vượt quá 100 ký tự.");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("Quận/Huyện không được để trống.")
                .MaximumLength(100).WithMessage("Quận/Huyện không được vượt quá 100 ký tự.");

            RuleFor(x => x.ProvinceCity)
                .NotEmpty().WithMessage("Tỉnh/Thành phố không được để trống.")
                .MaximumLength(100).WithMessage("Tỉnh/Thành phố không được vượt quá 100 ký tự.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Quốc gia không được để trống.")
                .MaximumLength(100).WithMessage("Quốc gia không được vượt quá 100 ký tự.");
        }
    }
}
