using Catalog.Application.Features.SeatMaps.Requests;
using FluentValidation;

namespace Catalog.Application.Features.SeatMaps.Validators
{
    public class CreateSeatMapRequestValidator : AbstractValidator<CreateSeatMapRequest>
    {
        public CreateSeatMapRequestValidator()
        {
            RuleFor(x => x.VenueId)
                .NotEmpty().WithMessage("Mã địa điểm không được để trống.");

            RuleFor(x => x.SeatMapName)
                .NotEmpty().WithMessage("Tên sơ đồ chỗ ngồi không được để trống.")
                .MaximumLength(200).WithMessage("Tên sơ đồ chỗ ngồi không được vượt quá 200 ký tự.");

            RuleFor(x => x.Width)
                .NotEmpty().WithMessage("Chiều rộng không được để trống.")
                .GreaterThan(0).WithMessage("Chiều rộng phải lớn hơn 0.");

            RuleFor(x => x.Height)
                .NotEmpty().WithMessage("Chiều cao không được để trống.")
                .GreaterThan(0).WithMessage("Chiều cao phải lớn hơn 0.");

            RuleFor(x => x.SvgFile)
                .NotNull().WithMessage("Tệp SVG không được để trống.")
                .Must(file => file.ContentType == "image/svg+xml").WithMessage("Tệp phải có định dạng SVG.");

            RuleForEach(x => x.Zones).SetValidator(new CreateZoneRequestValidator());
        }
    }
}
