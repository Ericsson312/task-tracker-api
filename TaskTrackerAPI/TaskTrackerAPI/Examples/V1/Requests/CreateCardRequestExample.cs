using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.V1.Requests
{
    public class CreateCardRequestExample : IExamplesProvider<CreateCardRequest>
    {
        public CreateCardRequest GetExamples()
        {
            return new CreateCardRequest
            {
                Name = "Card name",
                Tags = new List<string>
                {
                    "Tag name"
                }
            };
        }
    }
}