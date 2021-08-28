using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.V1.Requests
{
    public class UpdateCardRequestExample : IExamplesProvider<UpdateCardRequest>
    {
        public UpdateCardRequest GetExamples()
        {
            return new UpdateCardRequest
            {
                Name = "Card name",
            };
        }
    }
}