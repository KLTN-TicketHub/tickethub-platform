using Catalog.Application.Features.EventRatings.Requests;
using FluentValidation;

namespace Catalog.Application.Features.EventRatings.Validators
{
    public class CreateEventRatingRequestValidator : AbstractValidator<CreateEventRatingRequest>
    {
        public CreateEventRatingRequestValidator()
        {
            RuleFor(x => x.SoundRating)
                .InclusiveBetween(1, 5).WithMessage("Đánh giá âm thanh phải từ 1 đến 5 sao.");

            RuleFor(x => x.VisualRating)
                .InclusiveBetween(1, 5).WithMessage("Đánh giá ánh sáng/hình ảnh phải từ 1 đến 5 sao.");

            RuleFor(x => x.OrganizationRating)
                .InclusiveBetween(1, 5).WithMessage("Đánh giá tổ chức/sắp xếp phải từ 1 đến 5 sao.");

            RuleFor(x => x.FacilityRating)
                .InclusiveBetween(1, 5).WithMessage("Đánh giá trang thiết bị/cơ sở vật chất phải từ 1 đến 5 sao.");

            RuleFor(x => x.ServiceRating)
                .InclusiveBetween(1, 5).WithMessage("Đánh giá nhân viên/dịch vụ phải từ 1 đến 5 sao.");

            RuleFor(x => x.PerformanceRating)
                .InclusiveBetween(1, 5).WithMessage("Đánh giá nghệ sĩ/chương trình biểu diễn phải từ 1 đến 5 sao.");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Bình luận không được vượt quá 1000 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Comment));
        }
    }
}
