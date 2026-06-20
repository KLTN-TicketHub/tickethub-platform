using Catalog.Application.Features.Events.Requests;
using FluentValidation;

namespace Catalog.Application.Features.Events.Validators
{
    public class CreateShowTimeRequestValidator : AbstractValidator<CreateShowTimeRequest>
    {
        public CreateShowTimeRequestValidator()
        {
            RuleFor(x => x.StartAt)
                .NotEmpty()
                .WithMessage("Thời gian bắt đầu suất chiếu không được để trống.")
                .Must(startAt => startAt > DateTime.UtcNow)
                .WithMessage("Thời gian bắt đầu suất chiếu phải lớn hơn thời điểm hiện tại.");

            RuleFor(x => x.EndAt)
                .NotEmpty()
                .WithMessage("Thời gian kết thúc suất chiếu không được để trống.")
                .GreaterThan(x => x.StartAt)
                .WithMessage("Thời gian kết thúc suất chiếu phải lớn hơn thời gian bắt đầu.");

            RuleFor(x => x.TicketTypes)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Danh sách loại vé trong suất chiếu không được để trống.")
                .NotEmpty()
                .WithMessage("Mỗi suất chiếu phải có ít nhất một loại vé.");

            RuleFor(x => x.TicketTypes)
                .Must(ticketTypes =>
                    ticketTypes == null ||
                    ticketTypes.Select(t => t.TicketTypeName)
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .Count() == ticketTypes.Count)
                .WithMessage("Tên loại vé phải là duy nhất trong cùng một suất chiếu.");

            RuleFor(x => x.TicketTypes)
                .Must((request, ticketTypes) =>
                {
                    if (ticketTypes == null) return true;

                    List<Guid?> allZoneIds = ticketTypes
                        .Where(t => t.ZoneId != null)
                        .Select(t => t.ZoneId!)
                        .ToList();

                    return allZoneIds.Distinct().Count() == allZoneIds.Count;
                }).WithMessage("Một ZoneId không được phép xuất hiện ở nhiều loại vé khác nhau trong cùng một suất chiếu.");

            RuleForEach(x => x.TicketTypes)
                .SetValidator(new CreateTicketTypeRequestValidator());
        }
    }
}
