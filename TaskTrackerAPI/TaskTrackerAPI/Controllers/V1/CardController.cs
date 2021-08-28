using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Examples.V1.Responses;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class CardController : Controller
    {
        private readonly IBoardService _boardService;
        
        public CardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        /// <summary>
        /// Returns all the cards in the system
        /// </summary>
        /// <response code="200">Returns all the cards in the system</response>
        [HttpGet(ApiRoutes.Cards.GetAll)]
        [ProducesResponseType(typeof(CardResponse), 200)]
        public async Task<IActionResult> GetAllAsync()
        {
            var cards = await _boardService.GetCardsAsync();
            var cardResponse = cards.Select(x => new CardResponse
            {
                Id = x.Id,
                Name = x.Name,
                UserId = x.UserId,
                Tags = x.Tags.Select(xx => new TagResponse { Name = xx.TagName }).ToList()
            }).ToList();

            return Ok(new Response<List<CardResponse>>(cardResponse));
        }

        /// <summary>
        /// Returns a certain card in the system
        /// </summary>
        /// <response code="200">Returns a certain card in the system</response>
        /// <response code="404">Unable to find card</response>
        [HttpGet(ApiRoutes.Cards.Get)]
        [ProducesResponseType(typeof(CardResponse), 200)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid cardId)
        {
            var card = await _boardService.GetCardByIdAsync(cardId);

            if (card == null)
            {
                return NotFound();
            }

            return Ok(new Response<CardResponse>(new CardResponse 
            {
                Id = card.Id,
                Name = card.Name,
                UserId = card.UserId,
                Tags = card.Tags.Select(x => new TagResponse { Name = x.TagName }).ToList()
            }));
        }

        /// <summary>
        /// Creates new card for the board in the system
        /// </summary>
        /// <response code="201">Creates new card for the board in the system</response>
        /// <response code="404">Unable to find board for new card</response>
        /// <response code="400">Unable to create card due to validation error</response>
        [HttpPost(ApiRoutes.Cards.Create)]
        [ProducesResponseType(typeof(CardResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> CreateAsync([FromHeader] Guid boardId, [FromBody] CreateCardRequest cardRequest)
        {
            var board = await _boardService.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to find board for new card" } }
                });
            }
            
            var userBelongsToBoard = await _boardService.UserBelongsToBoard(boardId, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to create card due to validation error" } }
                });
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

            return Created(location, new Response<CardResponse>(response));
        }
        
        /// <summary>
        /// Updates name of the selected card
        /// </summary>
        /// <response code="200">Updates name of the selected card</response>
        /// <response code="404">Unable to find card</response>
        /// <response code="400">Unable to update card due to validation error</response>
        [HttpPut(ApiRoutes.Cards.Update)]
        [ProducesResponseType(typeof(CardResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid cardId, [FromBody] UpdateCardRequest cardRequest)
        {
            var cardToUpdate = await _boardService.GetCardByIdAsync(cardId);

            if (cardToUpdate == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to find card to update" } }
                });
            }
            
            var userBelongsToBoard = await _boardService.UserBelongsToBoard(cardToUpdate.BoardId, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to update card due to validation error" } }
                });
            }
            
            cardToUpdate.Name = cardRequest.Name;

            var updated = await _boardService.UpdateCardAsync(cardToUpdate);

            if (updated)
            {
                return Ok(new Response<CardResponse>(new CardResponse
                {
                    Id = cardToUpdate.Id,
                    Name = cardToUpdate.Name,
                    UserId = cardToUpdate.UserId,
                    Tags = cardToUpdate.Tags?.Select(x => new TagResponse { Name = x.TagName }).ToList()
                }));
            }

            return NotFound();
        }

        /// <summary>
        /// Deletes card from the board from the system
        /// </summary>
        /// <response code="204">Deletes card from the board from the system</response>
        /// <response code="404">Unable to find card</response>
        /// <response code="400">Unable to delete card due to validation error</response>
        [HttpDelete(ApiRoutes.Cards.Delete)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid cardId)
        {
            var cardToDelete = await _boardService.GetCardByIdAsync(cardId);

            if (cardToDelete == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to find card" } }
                });
            }
            
            var userBelongsToBoard = await _boardService.UserBelongsToBoard(cardToDelete.BoardId, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to delete card due to validation error" } }
                });
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
