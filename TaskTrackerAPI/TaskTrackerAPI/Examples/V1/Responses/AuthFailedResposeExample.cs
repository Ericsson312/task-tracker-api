using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Responses;

namespace TaskTrackerApi.Examples.V1.Responses
{
    public class AuthFailedResponseExample : IExamplesProvider<AuthFailedResponse>
    {
        public AuthFailedResponse GetExamples()
        {
            return new AuthFailedResponse
            {
                Errors = new List<string>
                {
                    "Auth failed"
                }
            };
        }
    }
}