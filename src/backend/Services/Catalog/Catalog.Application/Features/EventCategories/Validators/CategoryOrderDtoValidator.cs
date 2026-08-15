using Catalog.Application.Features.EventCategories.Commands.ReorderEventCategories;
using FluentValidation;

namespace Catalog.Application.Features.EventCategories.Validators
{
    public class CategoryOrderDtoValidator : AbstractValidator<CategoryOrderDto>
    {
        public CategoryOrderDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("ID danh mục không được để trống.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải lớn hơn hoặc bằng 0.");
        }
    }
}
