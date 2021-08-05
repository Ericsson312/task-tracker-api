using FluentValidation;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Validators
{
    public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequest>
    {
        public CreateMemberRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();
        }
    }
}