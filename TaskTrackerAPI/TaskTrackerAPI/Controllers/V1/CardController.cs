using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1.Requiests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CardController : Controller
    {
        private readonly ICardService _cardService;
        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet(ApiRoutes.Cards.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            var card = await _cardService.GetCardsAsync();
            var cardResponse = card.Select(x => new CardResponse
            {
                Id = x.Id,
                Name = x.Name,
                UserId = x.UserId,
                Tags = x.Tags.Select(xx => new TagResponse { Name = xx.TagName }).ToList()
            });

            return Ok(cardResponse);
        }

        [HttpGet(ApiRoutes.Cards.Get)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid cardId)
        {
            var card = await _cardService.GetCardByIdAsync(cardId);

            if (card == null)
            {
                return NotFound();
            }

            return Ok(new CardResponse 
            {
                Id = card.Id,
                Name = card.Name,
                UserId = card.UserId,
                Tags = card.Tags.Select(x => new TagResponse { Name = x.TagName }).ToList()
            });
        }

        [HttpPut(ApiRoutes.Cards.Update)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid cardId, [FromBody] UpdateCardRequest cardRequest)
        {
            var userOwnsCard = await _cardService.UserOwnsCardAsync(cardId, HttpContext.GetUserId());

            if (!userOwnsCard)
            {
                return BadRequest(new { error = "You do not own this card" });
            }

            var cardToUpdate = await _cardService.GetCardByIdAsync(cardId);
            cardToUpdate.Name = cardRequest.Name;

            var updated = await _cardService.UpdateCardAsync(cardToUpdate);

            if (updated)
            {
                return Ok(new CardResponse
                {
                    Id = cardToUpdate.Id,
                    Name = cardToUpdate.Name,
                    UserId = cardToUpdate.UserId,
                    Tags = cardToUpdate.Tags.Select(x => new TagResponse { Name = x.TagName }).ToList()
                });
            }

            return NotFound();
        }

        [HttpPost(ApiRoutes.Cards.Create)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCardRequest cardRequest)
        {
            var newCardId = Guid.NewGuid();

            var card = new Card
            {
                Id = newCardId,
                Name = cardRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = cardRequest.Tags.Select(tagName => new CardTag { TagName = tagName, CardId = newCardId }).ToList()
            };

            await _cardService.CreateCardAsync(card);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Cards.Get.Replace("{cardId}", card.Id.ToString())}";

            var response = new CardResponse 
            { 
                Id = card.Id,
                Name = card.Name,
                UserId = card.UserId,
                Tags = card.Tags.Select(x => new TagResponse { Name = x.TagName }).ToList()
            };

            return Created(location, response);
        }

        [HttpDelete(ApiRoutes.Cards.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid taskId)
        {
            var userOwnsCard = await _cardService.UserOwnsCardAsync(taskId, HttpContext.GetUserId());

            if (!userOwnsCard)
            {
                return BadRequest(new { error = "You do not own this card" });
            }

            var deleted = await _cardService.DeleteCardAsync(taskId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
