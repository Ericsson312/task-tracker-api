using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApi;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTracker_IntegrationTest
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient _testClient;
        protected readonly IServiceProvider _serviceProvider;

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
            _serviceProvider = appFactory.Services;
            _testClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            _testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<CardResponse> CreateAsync(CreateCardRequest request)
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Cards.Create, request);
            return await response.Content.ReadAsAsync<CardResponse>();
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "adam@tasker.com",
                Password = "Adam12345!"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return registrationResponse.Token;
        }

        public void Dispose()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();
        }
    }
}
