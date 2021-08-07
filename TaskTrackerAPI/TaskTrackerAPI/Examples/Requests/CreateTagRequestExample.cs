using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.Requests
{
    public class CreateTagRequestExample : IExamplesProvider<CreateTagRequest>
    {
        public CreateTagRequest GetExamples()
        {
            return new CreateTagRequest
            {
                Name = "new fancy tag"
            };
        }
    }
}