using Catalog.Application.Features.SeatMaps.Requests;
using FluentValidation;

namespace Catalog.Application.Features.SeatMaps.Validators
{
    public class CreateSvgElementRequestValidator : AbstractValidator<CreateSvgElementRequest>
    {
        public CreateSvgElementRequestValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Loại phần tử SVG không được để trống.")
                .MaximumLength(50).WithMessage("Loại phần tử SVG không được vượt quá 50 ký tự.");

            RuleFor(x => x.X)
                .GreaterThanOrEqualTo(0).WithMessage("Tọa độ X phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Y)
                .GreaterThanOrEqualTo(0).WithMessage("Tọa độ Y phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Width)
                .GreaterThanOrEqualTo(0).WithMessage("Chiều rộng phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Height)
                .GreaterThanOrEqualTo(0).WithMessage("Chiều cao phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Fill)
                .MaximumLength(100).WithMessage("Màu tô không được vượt quá 100 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Fill));

            RuleFor(x => x.Stroke)
                .MaximumLength(100).WithMessage("Màu viền không được vượt quá 100 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Stroke));

            RuleFor(x => x.StrokeWidth)
                .GreaterThanOrEqualTo(0).WithMessage("Độ dày viền phải lớn hơn hoặc bằng 0.")
                .When(x => x.StrokeWidth.HasValue);

            RuleFor(x => x.Data)
                .MaximumLength(2000).WithMessage("Dữ liệu nét vẽ không được vượt quá 2000 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Data));

            RuleFor(x => x.Text)
                .MaximumLength(500).WithMessage("Nội dung văn bản không được vượt quá 500 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Text));

            RuleFor(x => x.FontSize)
                .GreaterThanOrEqualTo(0).WithMessage("Kích thước chữ phải lớn hơn hoặc bằng 0.")
                .When(x => x.FontSize.HasValue);

            RuleFor(x => x.FontFamily)
                .MaximumLength(100).WithMessage("Font chữ không được vượt quá 100 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.FontFamily));
        }
    }
}
