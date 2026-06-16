using Catalog.Application.Features.Events.Requests;
using FluentValidation;

namespace Catalog.Application.Features.Events.Validators
{
    public class CreateTicketTypeRequestValidator : AbstractValidator<CreateTicketTypeRequest>
    {
        public CreateTicketTypeRequestValidator()
        {
            RuleFor(x => x.TicketTypeName)
                .NotEmpty().WithMessage("Tên loại vé không được để trống.")
                .MaximumLength(200).WithMessage("Tên loại vé không được vượt quá 200 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả loại vé không được vượt quá 500 ký tự.");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Giá vé không được để trống.")
                .GreaterThanOrEqualTo(0).WithMessage("Giá vé phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.PublishedQuota)
                .GreaterThan(0)
                .WithMessage("Số lượng vé phát hành phải lớn hơn 0.");

            RuleFor(x => x.MinQtyQuota)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng vé tối thiểu phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.MaxQtyQuota)
                .GreaterThanOrEqualTo(x => x.MinQtyQuota).WithMessage("Số lượng vé tối đa phải lớn hơn hoặc bằng số lượng vé tối thiểu.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải lớn hơn hoặc bằng 0.");
        }
    }
}
