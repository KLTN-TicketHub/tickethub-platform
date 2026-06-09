using Catalog.Application.Features.SeatMaps.Requests;
using FluentValidation;

namespace Catalog.Application.Features.SeatMaps.Validators
{
    public class CreateRowRequestValidator : AbstractValidator<CreateRowRequest>
    {
        public CreateRowRequestValidator()
        {
            RuleFor(x => x.RowLabel)
                .NotEmpty().WithMessage("Tên hàng ghế không được để trống.")
                .MaximumLength(20).WithMessage("Tên hàng ghế không được vượt quá 20 ký tự.");

            RuleFor(x => x.SeatRequests)
                .NotEmpty().WithMessage("Danh sách ghế không được để trống.");

            RuleForEach(x => x.SeatRequests)
                .SetValidator(new CreateSeatRequestValidator());
        }
    }
}
