using Catalog.Application.Features.EventCategories.Commands.ReorderEventCategories;
using FluentValidation;

namespace Catalog.Application.Features.EventCategories.Validators
{
    public class ReorderEventCategoriesRequestValidator : AbstractValidator<List<CategoryOrderDto>>
    {
        public ReorderEventCategoriesRequestValidator()
        {
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Danh sách thứ tự danh mục không được để trống.");

            RuleFor(x => x)
                .Must(categories => categories.Select(c => c.CategoryId).Distinct().Count() == categories.Count)
                .WithMessage("Danh sách chứa ID danh mục bị trùng lặp.")
                .When(x => x.Count > 0);

            RuleForEach(x => x).SetValidator(new CategoryOrderDtoValidator());
        }
    }
}
