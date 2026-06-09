using Catalog.Application.Features.SeatMaps.Requests;
using FluentValidation;

namespace Catalog.Application.Features.SeatMaps.Validators
{
    public class CreateSeatRequestValidator : AbstractValidator<CreateSeatRequest>
    {
        public CreateSeatRequestValidator()
        {
            RuleFor(x => x.SeatName)
                .NotEmpty().WithMessage("Tên ghế không được để trống.")
                .MaximumLength(50).WithMessage("Tên ghế không được vượt quá 50 ký tự.");

            RuleFor(x => x.SvgElementId)
                .NotEmpty().WithMessage("ID phần tử SVG không được để trống.")
                .MaximumLength(50).WithMessage("ID phần tử SVG không được vượt quá 50 ký tự.");

            RuleFor(x => x.X)
                .GreaterThanOrEqualTo(0).WithMessage("Tọa độ X phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Y)
                .GreaterThanOrEqualTo(0).WithMessage("Tọa độ Y phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Radius)
                .GreaterThan(0).WithMessage("Bán kính ghế phải lớn hơn 0.");
        }
    }
}
