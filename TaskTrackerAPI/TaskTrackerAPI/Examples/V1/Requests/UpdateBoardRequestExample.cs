using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.V1.Requests
{
    public class UpdateBoardRequestExample : IExamplesProvider<UpdateBoardRequest>
    {
        public UpdateBoardRequest GetExamples()
        {
            return new UpdateBoardRequest
            {
                Name = "New name",
                Description = "New description"
            };
        }
    }
}