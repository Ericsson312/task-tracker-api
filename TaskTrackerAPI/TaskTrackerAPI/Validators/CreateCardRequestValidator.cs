using FluentValidation;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Validators
{
    public class CreateCardRequestValidator : AbstractValidator<CreateCardRequest>
    {
        public CreateCardRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}