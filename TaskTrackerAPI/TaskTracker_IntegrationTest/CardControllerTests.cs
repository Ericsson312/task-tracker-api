using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Domain;
using Xunit;

namespace TaskTracker_IntegrationTest
{
    public class CardControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyCard_ReturnEmptyResponse()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var response = await _testClient.GetAsync(ApiRoutes.Cards.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Card>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_WhenCardExistsInTheDatabase_ReturnsCard()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var createdCard = await CreateAsync(new CreateCardRequest
            {
                Name = "Clean the car."
            });

            var response = await _testClient.GetAsync(ApiRoutes.Cards.Get.Replace("{taskId}", createdCard.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedCard = await response.Content.ReadAsAsync<Card>();
            returnedCard.Id.Should().Be(createdCard.Id);
            returnedCard.Name.Should().Be("Clean the car.");
        }

        [Fact]
        public async Task Delete_WhenCardExistsInDatabase_ReturnsNoContentStatusCode()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var createdCard = await CreateAsync(new CreateCardRequest
            {
                Name = "Clean the car."
            });

            var deletedCardResponse = await _testClient.DeleteAsync(ApiRoutes.Cards.Delete.Replace("{cardId}", createdCard.Id.ToString()));
            var getDeletedCardResponse = await _testClient.GetAsync(ApiRoutes.Cards.Get.Replace("{cardId}", createdCard.Id.ToString()));

            // Assert
            deletedCardResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getDeletedCardResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WhenCardExistsInDatabase_ReturnsUpdatedCard()
        {
            // Arrage
            await AuthenticateAsync();

            // Act
            var createdCard = await CreateAsync(new CreateCardRequest { Name = "Clean the car." });
            var response = await _testClient.PutAsJsonAsync(ApiRoutes.Cards.Update.Replace("{cardId}", createdCard.Id.ToString()), new UpdateCardRequest
            {
                Name = "Clean the room."
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedCard = await response.Content.ReadAsAsync<Card>();
            updatedCard.Name.Should().Be("Clean the room.");
            createdCard.Id.Should().Be(updatedCard.Id);
        }

    }
}
