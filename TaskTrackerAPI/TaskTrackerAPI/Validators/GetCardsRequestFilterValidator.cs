using FluentValidation;
using TaskTrackerApi.Examples.V1.Requests.Queries;

namespace TaskTrackerApi.Validators
{
    public class GetCardsRequestFilterValidator : AbstractValidator<PaginationQuery>
    {
        public GetCardsRequestFilterValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .GreaterThan(0);
        }
    }
}