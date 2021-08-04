using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CardController : Controller
    {
        private readonly IBoardService _boardService;
        
        public CardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet(ApiRoutes.Cards.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            var cards = await _boardService.GetCardsAsync();
            var cardResponse = cards.Select(x => new CardResponse
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
            var card = await _boardService.GetCardByIdAsync(cardId);

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
            var cardToUpdate = await _boardService.GetCardByIdAsync(cardId);

            if (cardToUpdate == null)
            {
                return NotFound(new ErrorResponse(new ErrorModel{ Message = "Invalid card id"}));
            }

            var board = await _boardService.GetBoardByCardIdAsync(cardId);
            var userBelongsToBoard = await _boardService.UserBelongsToBoard(board.Id, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse(new ErrorModel{ Message = "You do not have permission to change this card"}));
            }
            
            cardToUpdate.Name = cardRequest.Name;

            var updated = await _boardService.UpdateCardAsync(cardToUpdate);

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
        public async Task<IActionResult> CreateAsync([FromHeader] Guid boardId, [FromBody] CreateCardRequest cardRequest)
        {
            var board = await _boardService.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return NotFound(new ErrorResponse(new ErrorModel{ Message = "Invalid board id"}));
            }
            
            var userBelongsToBoard = await _boardService.UserBelongsToBoard(boardId, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse(new ErrorModel{ Message = "You do not have permission to add new card"}));
            }
            
            var newCardId = Guid.NewGuid();
            var userId = HttpContext.GetUserId();

            var card = new Card
            {
                Id = newCardId,
                UserId = userId,
                BoardId = boardId,
                Name = cardRequest.Name,
                Tags = cardRequest.Tags?.Select(tagName => new CardTag { TagName = tagName, CardId = newCardId }).ToList()
            };

            await _boardService.CreateCardAsync(card);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Cards.Get.Replace("{cardId}", card.Id.ToString())}";

            var response = new CardResponse 
            { 
                Id = card.Id,
                Name = card.Name,
                UserId = card.UserId,
                Tags = card.Tags?.Select(x => new TagResponse { Name = x.TagName }).ToList()
            };

            return Created(location, response);
        }

        [HttpDelete(ApiRoutes.Cards.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid cardId)
        {
            var board = await _boardService.GetBoardByCardIdAsync(cardId);
            var userBelongsToBoard = await _boardService.UserBelongsToBoard(board.Id, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse(new ErrorModel{ Message = "You do not have permission to change this card"}));
            }

            var deleted = await _boardService.DeleteCardAsync(cardId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
