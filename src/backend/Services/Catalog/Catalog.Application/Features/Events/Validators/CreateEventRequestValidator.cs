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

        RuleFor(x => x.StartAt)
            .NotEmpty()
            .WithMessage("Thời gian bắt đầu không được để trống.")
            .Must(startAt => startAt > DateTime.UtcNow)
            .WithMessage("Thời gian bắt đầu phải lớn hơn thời điểm hiện tại.");

        RuleFor(x => x.EndAt)
            .NotEmpty()
            .WithMessage("Thời gian kết thúc không được để trống.")
            .GreaterThan(x => x.StartAt)
            .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");

        RuleFor(x => x.SaleOpenAt)
            .NotEmpty()
            .WithMessage("Thời gian mở bán không được để trống.")
            .LessThan(x => x.StartAt)
            .WithMessage("Thời gian mở bán phải nhỏ hơn thời gian bắt đầu sự kiện.");

        RuleFor(x => x.SaleCloseAt)
            .NotEmpty()
            .WithMessage("Thời gian đóng bán không được để trống.")
            .GreaterThan(x => x.SaleOpenAt)
            .WithMessage("Thời gian đóng bán phải lớn hơn thời gian mở bán.")
            .LessThan(x => x.EndAt)
            .WithMessage("Thời gian đóng bán phải nhỏ hơn thời gian kết thúc sự kiện.");

        RuleFor(x => x.CoverImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("URL ảnh bìa không được để trống.")
            .MaximumLength(500)
            .WithMessage("URL ảnh bìa không được vượt quá 500 ký tự.");

        RuleFor(x => x.TicketTypes)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Danh sách loại vé không được để trống.")
            .NotEmpty()
            .WithMessage("Danh sách loại vé không được để trống.");

        RuleFor(x => x.TicketTypes)
            .Must(ticketTypes =>
                ticketTypes == null ||
                ticketTypes.Select(t => t.TicketTypeName)
                           .Distinct(StringComparer.OrdinalIgnoreCase)
                           .Count() == ticketTypes.Count)
            .WithMessage("Tên loại vé phải là duy nhất trong danh sách.");

        RuleFor(x => x.Location)
            .Must((request, location) =>
                (request.SeatMapId == null && location != null) ||
                (request.SeatMapId != null && location == null))
            .WithMessage("Phải cung cấp Location hoặc SeatMapId, không được đồng thời cả hai.");

        RuleFor(x => x.TicketTypes)
            .Must(ticketTypes =>
                ticketTypes == null ||
                ticketTypes.All(t => t.ZoneId != null))
            .When(x => x.SeatMapId != null)
            .WithMessage("Mỗi loại vé phải có mã khu vực khi sử dụng sơ đồ chỗ ngồi.");

        RuleFor(x => x.TicketTypes)
            .Must(ticketTypes =>
            {
                if (ticketTypes == null) return true;

                List<Guid?> allZoneIds = ticketTypes
                    .Where(t => t.ZoneId != null)
                    .Select(t => t.ZoneId!)
                    .ToList();

                return allZoneIds.Distinct().Count() == allZoneIds.Count;
            }).WithMessage("Một ZoneId không được phép xuất hiện ở nhiều loại vé khác nhau.");

        RuleFor(x => x.TicketTypes)
            .Must((request, ticketTypes) =>
            {
                if (request.SeatMapId != null) return true;

                return ticketTypes == null ||
                    ticketTypes.All(t => t.ZoneId == null);
            }).WithMessage("Không được phép cung cấp ZoneId khi sự kiện không sử dụng sơ đồ chỗ ngồi (SeatMap).");

        RuleForEach(x => x.TicketTypes)
            .SetValidator(new CreateTicketTypeRequestValidator());

        RuleFor(x => x.Location)
            .SetValidator(new CreateLocationRequestValidator()!)
            .When(x => x.Location != null);
    }
}