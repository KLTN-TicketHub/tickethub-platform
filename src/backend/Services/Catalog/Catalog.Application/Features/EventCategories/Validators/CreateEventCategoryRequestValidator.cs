using Catalog.Application.Features.EventCategories.Requests;
using FluentValidation;

namespace Catalog.Application.Features.EventCategories.Validators
{
    public class CreateEventCategoryRequestValidator : AbstractValidator<CreateEventCategoryRequest>
    {
        public CreateEventCategoryRequestValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Tên danh mục không được để trống.")
                .MaximumLength(50).WithMessage("Tên danh mục không được vượt quá 50 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
