using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Repositories;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BoardController : Controller
    {
        private readonly IBoardService _boardService;
        
        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }
        
        [HttpGet(ApiRoutes.Board.GetAll)]
        // [Authorize(Roles = "Admin")]
        // [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> GetAllAsync()
        {
            var boards =  await _boardService.GetBoardsAsync();
            
            var bordsResponse = new List<BoardResponse>();

            foreach (var board in boards)
            {
                bordsResponse.Add(new BoardResponse
                {
                    Id = board.Id,
                    UserId = board.UserId,
                    Name = board.Description,
                    Description = board.Description,
                    Cards = board.Cards?.Select(x => new CardResponse
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Name = x.Name,
                        Tags = new List<TagResponse>()
                    }).ToList()
                });
            }

            return Ok(bordsResponse);
        }
        
        [HttpGet(ApiRoutes.Board.Get)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid boardId)
        {
            var board =  await _boardService.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return NotFound();
            }
  
            return Ok(new BoardResponse
            {
                Id = board.Id,
                Name = board.Name,
                Description = board.Description,
                Cards = board.Cards.Select(x => new CardResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Tags = x.Tags.Select(xx => new TagResponse { Name = xx.TagName }).ToList()
                }).ToList()
            });
        }
        
        [HttpPost(ApiRoutes.Board.Create)]
        public async Task <IActionResult> CreateAsync([FromBody] CreateBoardRequest boardRequest)
        {
            var newBoardId = Guid.NewGuid();

            var board = new Board
            {
                Id = newBoardId,
                Name = boardRequest.Name,
                Description = boardRequest.Description,
                UserId = HttpContext.GetUserId(),
            };

            await _boardService.CreateBoardAsync(board);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Board.Get.Replace("{boardId}", board.Id.ToString())}";
            
            var response = new BoardResponse 
            { 
                Id = board.Id,
                UserId = board.UserId,
                Name = board.Name,
                Description = board.Description,
                Cards = new List<CardResponse>()
            };
            
            return Created(location, response);
        }
        
        [HttpDelete(ApiRoutes.Board.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid boardId)
        {
            var userOwnsBoard = await _boardService.UserOwnsBoardAsync(boardId, HttpContext.GetUserId());

            if (!userOwnsBoard)
            {
                return BadRequest(new ErrorResponse(new ErrorModel{ Message = "You do not own this post"}));
            }

            var deleted = await _boardService.DeleteBoardIdAsync(boardId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}