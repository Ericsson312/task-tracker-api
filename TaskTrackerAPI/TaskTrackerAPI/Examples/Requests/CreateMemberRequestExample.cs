﻿using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.Requests
{
    public class CreateMemberRequestExample : IExamplesProvider<CreateMemberRequest>
    {
        public CreateMemberRequest GetExamples()
        {
            return new CreateMemberRequest
            {
                Email = "newmember@example.com"
            };
        }
    }
}