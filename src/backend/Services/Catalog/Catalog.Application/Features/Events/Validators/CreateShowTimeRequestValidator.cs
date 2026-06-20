using Catalog.Application.Features.Events.Requests;
using FluentValidation;
using System;

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
        }
    }
}
