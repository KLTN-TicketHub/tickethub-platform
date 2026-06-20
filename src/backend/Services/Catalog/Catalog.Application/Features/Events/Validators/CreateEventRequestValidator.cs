using Catalog.Application.Features.Events.Requests;
using FluentValidation;

namespace Catalog.Application.Features.Events.Validators;

public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Mã danh mục không được để trống.");

        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Tiêu đề sự kiện không được để trống.")
            .MaximumLength(200)
            .WithMessage("Tiêu đề sự kiện không được vượt quá 200 ký tự.");

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Mô tả sự kiện không được để trống.")
            .MaximumLength(5000)
            .WithMessage("Mô tả sự kiện không được vượt quá 5000 ký tự.");

        RuleFor(x => x.ShowTimes)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Danh sách suất chiếu không được để trống.")
            .NotEmpty()
            .WithMessage("Sự kiện phải có ít nhất một suất chiếu.");

        RuleForEach(x => x.ShowTimes)
            .SetValidator(new CreateShowTimeRequestValidator());

        RuleFor(x => x.ShowTimes)
            .Must(showTimes =>
            {
                if (showTimes == null || showTimes.Count < 2) return true;

                var sorted = showTimes.OrderBy(s => s.StartAt).ToList();
                for (int i = 0; i < sorted.Count - 1; i++)
                {
                    if (sorted[i].EndAt > sorted[i + 1].StartAt)
                    {
                        return false;
                    }
                }
                return true;
            })
            .WithMessage("Thời gian của các suất chiếu không được trùng lặp hoặc đè lên nhau.");

        RuleFor(x => x.SaleOpenAt)
            .NotEmpty()
            .WithMessage("Thời gian mở bán không được để trống.")
            .Must((request, saleOpenAt) => request.ShowTimes == null || !request.ShowTimes.Any() || saleOpenAt < request.ShowTimes.Min(s => s.StartAt))
            .WithMessage("Thời gian mở bán phải nhỏ hơn thời gian bắt đầu suất chiếu sớm nhất.");

        RuleFor(x => x.SaleCloseAt)
            .NotEmpty()
            .WithMessage("Thời gian đóng bán không được để trống.")
            .GreaterThan(x => x.SaleOpenAt)
            .WithMessage("Thời gian đóng bán phải lớn hơn thời gian mở bán.")
            .Must((request, saleCloseAt) => request.ShowTimes == null || !request.ShowTimes.Any() || saleCloseAt < request.ShowTimes.Max(s => s.EndAt))
            .WithMessage("Thời gian đóng bán phải nhỏ hơn thời gian kết thúc suất chiếu muộn nhất.");

        RuleFor(x => x.CoverImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("URL ảnh bìa không được để trống.")
            .MaximumLength(500)
            .WithMessage("URL ảnh bìa không được vượt quá 500 ký tự.");

        RuleFor(x => x.Location)
            .Must((request, location) =>
                (request.SeatMapId == null && location != null) ||
                (request.SeatMapId != null && location == null))
            .WithMessage("Phải cung cấp Location hoặc SeatMapId, không được đồng thời cả hai.");

        RuleFor(x => x.Location)
            .SetValidator(new CreateLocationRequestValidator()!)
            .When(x => x.Location != null);
    }
}