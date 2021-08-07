using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Responses;

namespace TaskTrackerApi.Examples.Responses
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