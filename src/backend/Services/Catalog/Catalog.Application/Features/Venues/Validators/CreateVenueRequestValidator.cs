using Catalog.Application.Features.Venues.Requests;
using FluentValidation;

namespace Catalog.Application.Features.Venues.Validators
{
    public class CreateVenueRequestValidator : AbstractValidator<CreateVenueRequest>
    {
        public CreateVenueRequestValidator()
        {
            RuleFor(x => x.VenueName)
                .NotEmpty().WithMessage("Tên địa điểm không được để trống.")
                .MaximumLength(200).WithMessage("Tên địa điểm không được vượt quá 200 ký tự.");

            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Địa chỉ không được để trống.")
                .MaximumLength(200).WithMessage("Địa chỉ không được vượt quá 200 ký tự.");

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

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .MaximumLength(15).WithMessage("Số điện thoại không được vượt quá 15 ký tự.");

            RuleFor(x => x.WebsiteUrl)
                .MaximumLength(100).WithMessage("Địa chỉ website không được vượt quá 100 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));
        }
    }
}