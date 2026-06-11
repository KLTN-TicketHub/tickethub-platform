using Catalog.Application.Features.SeatMaps.Requests;
using FluentValidation;

namespace Catalog.Application.Features.SeatMaps.Validators
{
    public class CreateZoneRequestValidator : AbstractValidator<CreateZoneRequest>
    {
        public CreateZoneRequestValidator()
        {
            RuleFor(x => x.ZoneName)
                .NotEmpty().WithMessage("Tên khu vực không được để trống.")
                .MaximumLength(100).WithMessage("Tên khu vực không được vượt quá 100 ký tự.");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Màu sắc không được để trống.")
                .MaximumLength(20).WithMessage("Màu sắc không được vượt quá 20 ký tự.");

            RuleFor(x => x.X)
                .NotEmpty().WithMessage("Tọa độ X không được để trống.")
                .GreaterThanOrEqualTo(0).WithMessage("Tọa độ X phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Y)
                .NotEmpty().WithMessage("Tọa độ Y không được để trống.")
                .GreaterThanOrEqualTo(0).WithMessage("Tọa độ Y phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Width)
                .NotEmpty().WithMessage("Chiều rộng không được để trống.")
                .GreaterThan(0).WithMessage("Chiều rộng phải lớn hơn 0.");

            RuleFor(x => x.Height)
                .NotEmpty().WithMessage("Chiều cao không được để trống.")
                .GreaterThan(0).WithMessage("Chiều cao phải lớn hơn 0.");

            RuleFor(x => x.SvgElementId)
               .MaximumLength(50).WithMessage("ID phần tử SVG không được vượt quá 50 ký tự.")
               .When(x => !string.IsNullOrWhiteSpace(x.SvgElementId));

            RuleFor(x => x.Rows)
                .NotEmpty().WithMessage("Danh sách hàng không được để trống.")
                .When(x => x.IsReservingSeat);

            RuleFor(x => x.Rows)
                .Empty().WithMessage("Danh sách hàng phải trống khi khu vực không có ghế.")
                .When(x => !x.IsReservingSeat);

            RuleForEach(x => x.SvgElements)
                .SetValidator(new CreateSvgElementRequestValidator())
                .When(x => x.SvgElements != null);

            RuleForEach(x => x.Rows)
                .SetValidator(new CreateRowRequestValidator())
                .When(x => x.Rows != null);
        }
    }
}
