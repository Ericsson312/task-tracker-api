using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Responses;

namespace TaskTrackerApi.Examples.V1.Responses
{
    public class MemberResponseExample : IExamplesProvider<MemberResponse>
    {
        public MemberResponse GetExamples()
        {
            return new MemberResponse
            {
                Email = "member@example.com"
            };
        }
    }
}