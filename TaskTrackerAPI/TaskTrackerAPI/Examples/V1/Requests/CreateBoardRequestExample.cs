using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.V1.Requests
{
    public class CreateBoardRequestExample : IExamplesProvider<CreateBoardRequest>
    {
        public CreateBoardRequest GetExamples()
        {
            return new CreateBoardRequest
            {
                Name = "Board name",
                Description = "Board description"
            };
        }
    }
}