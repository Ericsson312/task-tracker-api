using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
        
        [HttpGet(ApiRoutes.Boards.GetAll)]
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Moderator")]
        public async Task<IActionResult> GetAllAsync()
        {
            var boards =  await _boardService.GetBoardsAsync();
            
            var boardResponses = new List<BoardResponse>();

            foreach (var board in boards)
            {
                boardResponses.Add(new BoardResponse
                {
                    Id = board.Id,
                    UserId = board.UserId,
                    Name = board.Name,
                    Description = board.Description,
                    Cards = board.Cards?.Select(x => new CardResponse
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Name = x.Name,
                        Tags = new List<TagResponse>()
                    }).ToList(),
                    Members = board.Members?.Select(x => new MemberResponse
                    {
                        Email = x.MemberEmail
                    }).ToList()
                });
            }

            return Ok(boardResponses);
        }
        
        [HttpGet(ApiRoutes.Boards.Get)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid boardId)
        {
            var board =  await _boardService.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return NotFound();
            }

            var userBelongsToBoard = await _boardService.UserBelongsToBoard(boardId, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse(new ErrorModel{ Message = "You do not belong to the board"}));
            }

            return Ok(new BoardResponse
            {
                Id = board.Id,
                UserId = board.UserId,
                Name = board.Name,
                Description = board.Description,
                Cards = board.Cards.Select(x => new CardResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Tags = x.Tags.Select(xx => new TagResponse{ Name = xx.TagName }).ToList()
                }).ToList(),
                Members = board.Members.Select(x => new MemberResponse{ Email = x.MemberEmail }).ToList()
            });
        }
        
        [HttpPut(ApiRoutes.Boards.Update)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid boardId, [FromBody] UpdateBoardRequest boardRequest)
        {
            var userOwnsBoard = await _boardService.UserOwnsBoardAsync(boardId, HttpContext.GetUserId());

            if (!userOwnsBoard)
            {
                return BadRequest(new ErrorResponse(new ErrorModel{ Message = "You do not own this board"}));
            }

            var boardToUpdate = await _boardService.GetBoardByIdAsync(boardId);
            boardToUpdate.Name = boardRequest.Name;
            boardToUpdate.Description = boardRequest.Description;

            var updated = await _boardService.UpdateBoardAsync(boardToUpdate);

            if (updated)
            {
                return Ok(new BoardResponse
                {
                    Id = boardToUpdate.Id,
                    UserId = boardToUpdate.UserId,
                    Name = boardToUpdate.Name,
                    Description = boardToUpdate.Description,
                    Cards = boardToUpdate.Cards.Select(x => new CardResponse
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Name = x.Name,
                        Tags = x.Tags.Select(xx => new TagResponse{ Name = xx.TagName }).ToList()
                    }).ToList(),
                    Members = boardToUpdate.Members.Select(xx => new MemberResponse{ Email = xx.MemberEmail }).ToList()
                });
            }

            return NotFound();
        }
        
        [HttpPost(ApiRoutes.Boards.Create)]
        public async Task <IActionResult> CreateAsync([FromBody] CreateBoardRequest boardRequest)
        {
            var newBoardId = Guid.NewGuid();

            var board = new Board
            {
                Id = newBoardId,
                Name = boardRequest.Name,
                Description = boardRequest.Description,
                UserId = HttpContext.GetUserId()
            };

            await _boardService.CreateBoardAsync(HttpContext.GetUserEmail(), board);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Boards.Get.Replace("{boardId}", board.Id.ToString())}";
            
            var response = new BoardResponse 
            { 
                Id = board.Id,
                UserId = board.UserId,
                Name = board.Name,
                Description = board.Description,
                Cards = new List<CardResponse>(),
                Members = new List<MemberResponse>()
            };
            
            return Created(location, response);
        }
        
        [HttpDelete(ApiRoutes.Boards.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid boardId)
        {
            var userOwnsBoard = await _boardService.UserOwnsBoardAsync(boardId, HttpContext.GetUserId());

            if (!userOwnsBoard)
            {
                return BadRequest(new ErrorResponse(new ErrorModel{ Message = "You do not own this board"}));
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