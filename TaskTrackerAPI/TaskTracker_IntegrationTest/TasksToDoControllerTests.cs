using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1.Requiests;
using TaskTrackerApi.Domain;
using Xunit;

namespace TaskTracker_IntegrationTest
{
    public class TasksToDoControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyTasksToDo_ReturnEmptyResponse()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var response = await _testClient.GetAsync(ApiRoutes.Tasks.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<TaskToDo>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_WhenTaskToDoExistsInTheDatabase_ReturnsTaskToDo()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var createdTaskToDo = await CreateAsync(new CreateTaskToDoRequest
            {
                Name = "Clean the car."
            });

            var response = await _testClient.GetAsync(ApiRoutes.Tasks.Get.Replace("{taskId}", createdTaskToDo.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedTaskToDo = await response.Content.ReadAsAsync<TaskToDo>();
            returnedTaskToDo.Id.Should().Be(createdTaskToDo.Id);
            returnedTaskToDo.Name.Should().Be("Clean the car.");
        }

        [Fact]
        public async Task Delete_WhenTaskToDoExistsInDatabase_ReturnsNoContentStatusCode()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var createdTaskToDo = await CreateAsync(new CreateTaskToDoRequest
            {
                Name = "Clean the car."
            });

            var deletedTaskToDoResponse = await _testClient.DeleteAsync(ApiRoutes.Tasks.Delete.Replace("{taskId}", createdTaskToDo.Id.ToString()));
            var getDeletedTaskToDoResponse = await _testClient.GetAsync(ApiRoutes.Tasks.Get.Replace("{taskId}", createdTaskToDo.Id.ToString()));

            // Assert
            deletedTaskToDoResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getDeletedTaskToDoResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WhenTaskToDoExistsInDatabase_ReturnsUpdatedTaskToDo()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var createdTaskToDo = await CreateAsync(new CreateTaskToDoRequest { Name = "Clean the car." });
            var response = await _testClient.PutAsJsonAsync(ApiRoutes.Tasks.Update.Replace("{taskId}", createdTaskToDo.Id.ToString()), new UpdateTaskToDoRequest
            {
                Name = "Clean the room."
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedTaskToDo = await response.Content.ReadAsAsync<TaskToDo>();
            updatedTaskToDo.Name.Should().Be("Clean the room.");
            createdTaskToDo.Id.Should().Be(updatedTaskToDo.Id);
        }

    }
}
