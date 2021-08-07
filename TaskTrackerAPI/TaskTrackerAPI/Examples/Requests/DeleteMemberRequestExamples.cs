using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.Requests
{
    public class DeleteMemberRequestExamples : IExamplesProvider<DeleteMemberRequest>
    {
        public DeleteMemberRequest GetExamples()
        {
            return new DeleteMemberRequest
            {
                Email = "member@example.com"
            };
        }
    }
}