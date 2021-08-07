using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Responses;

namespace TaskTrackerApi.Examples.Responses
{
    public class TagResponseExample : IExamplesProvider<TagResponse>
    {
        public TagResponse GetExamples()
        {
            return new TagResponse
            {
                Name = "new fancy tag"
            };
        }
    }
}