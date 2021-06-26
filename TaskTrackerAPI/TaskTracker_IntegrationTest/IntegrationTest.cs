using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApi;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1.Requiests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTracker_IntegrationTest
{
    public class IntegrationTest
    {
        protected readonly HttpClient _testClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });
            _testClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            _testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<TaskToDoResponse> CreateAsync(CreateTaskToDoRequest request)
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Tasks.Create, request);
            return await response.Content.ReadAsAsync<TaskToDoResponse>();
        }

        protected async Task<TaskToDo> UpdateAsync(Guid taskId, UpdateTaskToDoRequest request)
        {
            var response = await _testClient.PutAsJsonAsync(ApiRoutes.Tasks.Update.Replace("{taskId}", taskId.ToString()), request);
            return await response.Content.ReadAsAsync<TaskToDo>();
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@integration.com",
                Password = "Test12345!"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return registrationResponse.Token;
        }
    }
}
