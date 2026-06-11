using Catalog.Application.Features.SeatMaps.Requests;
using FluentValidation;

namespace Catalog.Application.Features.SeatMaps.Validators
{
    public class CreateSeatMapRequestValidator : AbstractValidator<CreateSeatMapRequest>
    {
        public CreateSeatMapRequestValidator()
        {
            RuleFor(x => x.SeatMapName)
                .NotEmpty().WithMessage("Tên sơ đồ chỗ ngồi không được để trống.")
                .MaximumLength(200).WithMessage("Tên sơ đồ chỗ ngồi không được vượt quá 200 ký tự.");

            RuleFor(x => x.Width)
                .NotEmpty().WithMessage("Chiều rộng không được để trống.")
                .GreaterThan(0).WithMessage("Chiều rộng phải lớn hơn 0.");

            RuleFor(x => x.Height)
                .NotEmpty().WithMessage("Chiều cao không được để trống.")
                .GreaterThan(0).WithMessage("Chiều cao phải lớn hơn 0.");

            RuleFor(x => x.SvgFileUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                .WithMessage("Đường dẫn tệp SVG không hợp lệ.")
                .When(x => x.SvgFileUrl != null);

            RuleFor(x => x.Zones)
                .NotEmpty().WithMessage("Danh sách khu vực không được để trống.");

            RuleForEach(x => x.Zones).SetValidator(new CreateZoneRequestValidator());
        }
    }
}
