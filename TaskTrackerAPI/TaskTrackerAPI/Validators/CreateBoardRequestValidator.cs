using FluentValidation;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Validators
{
    public class CreateBoardRequestValidator : AbstractValidator<CreateBoardRequest>
    {
        public CreateBoardRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
            
            RuleFor(x => x.Description)
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}