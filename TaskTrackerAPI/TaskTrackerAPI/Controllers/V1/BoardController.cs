using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class BoardController : Controller
    {
        private readonly IBoardService _boardService;
        
        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }
        
        /// <summary>
        /// Returns all the boards in the system
        /// </summary>
        /// <response code="200">Returns all the boards in the system</response>
        [HttpGet(ApiRoutes.Boards.GetAll)]
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Moderator")]
        [ProducesResponseType(typeof(BoardResponse), 200)]
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
                        Tags = x.Tags?.Select(xx => new TagResponse{ Name = xx.TagName }).ToList(),
                    }).ToList(),
                    Members = board.Members?.Select(x => new MemberResponse{ Email = x.MemberEmail }).ToList()
                });
            }

            return Ok(boardResponses);
        }
        
        /// <summary>
        /// Returns a certain board in the system
        /// </summary>
        /// <response code="200">Returns a certain board in the system</response>
        /// <response code="400">Unable to get board due to validation error</response>
        /// <response code="404">Unable to find board</response>
        [HttpGet(ApiRoutes.Boards.Get)]
        [ProducesResponseType(typeof(BoardResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid boardId)
        {
            var userBelongsToBoard = await _boardService.UserBelongsToBoard(boardId, HttpContext.GetUserEmail());

            if (!userBelongsToBoard)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to get board due to validation error" } }
                });
            }
            
            var board =  await _boardService.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return NotFound();
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
                    Tags = x.Tags?.Select(xx => new TagResponse{ Name = xx.TagName }).ToList()
                }).ToList(),
                Members = board.Members?.Select(x => new MemberResponse{ Email = x.MemberEmail }).ToList()
            });
        }
        
        /// <summary>
        /// Updates name and description of the selected board
        /// </summary>
        /// <response code="200">Updates name and description of the selected board</response>
        /// <response code="400">Unable to update board due to validation error</response>
        /// <response code="404">Unable to find board</response>
        [HttpPut(ApiRoutes.Boards.Update)]
        [ProducesResponseType(typeof(BoardResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid boardId, [FromBody] UpdateBoardRequest boardRequest)
        {
            var userId = HttpContext.GetUserId();
            
            var userOwnsBoard = await _boardService.UserOwnsBoardAsync(boardId, userId);

            if (!userOwnsBoard)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to update board due to validation error" } }
                });
            }

            var boardToUpdate = await _boardService.GetBoardByIdAsync(boardId);

            if (!string.IsNullOrEmpty(boardRequest.Name))
            {
                boardToUpdate.Name = boardRequest.Name;
            }

            if (!string.IsNullOrEmpty(boardRequest.Description))
            {
                boardToUpdate.Description = boardRequest.Description;
            }

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
        
        /// <summary>
        /// Creates new board in the system
        /// </summary>
        /// <response code="201">Creates new board in the system</response>
        [HttpPost(ApiRoutes.Boards.Create)]
        [ProducesResponseType(typeof(BoardResponse), 201)]
        public async Task <IActionResult> CreateAsync([FromBody] CreateBoardRequest boardRequest)
        {
            var newBoardId = Guid.NewGuid();
            var userId = HttpContext.GetUserId();
            var memberEmail = HttpContext.GetUserEmail();

            var board = new Board
            {
                Id = newBoardId,
                Name = boardRequest.Name,
                Description = boardRequest.Description,
                UserId = userId,
                Members = new List<BoardMember>
                {
                    new BoardMember{ MemberEmail = memberEmail, BoardId = newBoardId }
                }
            };

            await _boardService.CreateBoardAsync(board);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Boards.Get.Replace("{boardId}", board.Id.ToString())}";
            
            var response = new BoardResponse 
            { 
                Id = board.Id,
                UserId = board.UserId,
                Name = board.Name,
                Description = board.Description,
                Cards = new List<CardResponse>(),
                Members = board.Members.Select(x => new MemberResponse{ Email = x.MemberEmail })
            };
            
            return Created(location, response);
        }
        
        /// <summary>
        /// Deletes board from the system
        /// </summary>
        /// <response code="204">Deletes board from the system</response>
        /// <response code="400">Unable to delete board due to validation error</response>
        /// <response code="404">Unable to find board</response>
        [HttpDelete(ApiRoutes.Boards.Delete)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid boardId)
        {
            var userOwnsBoard = await _boardService.UserOwnsBoardAsync(boardId, HttpContext.GetUserId());

            if (!userOwnsBoard)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to delete board due to validation error" } }
                });
            }

            var deleted = await _boardService.DeleteBoardByIdAsync(boardId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}