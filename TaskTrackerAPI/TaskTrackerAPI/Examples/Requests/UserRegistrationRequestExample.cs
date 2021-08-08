using Swashbuckle.AspNetCore.Filters;
using TaskTrackerApi.Contracts.V1.Requests;

namespace TaskTrackerApi.Examples.Requests
{
    public class UserRegistrationRequestExample : IExamplesProvider<UserRegistrationRequest>
    {
        public UserRegistrationRequest GetExamples()
        {
            return new UserRegistrationRequest
            {
                Email = "register@example.com",
                Password = "User12345!"
            };
        }
    }
}