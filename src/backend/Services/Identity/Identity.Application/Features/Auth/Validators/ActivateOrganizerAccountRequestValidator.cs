using FluentValidation;
using Identity.Application.Features.Auth.Requests;

namespace Identity.Application.Features.Auth.Validators
{
    public class ActivateOrganizerAccountRequestValidator : AbstractValidator<ActivateOrganizerAccountRequest>
    {
        public ActivateOrganizerAccountRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId không được để trống.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token không được để trống.");
        }
    }
}
