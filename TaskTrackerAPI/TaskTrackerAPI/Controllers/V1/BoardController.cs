using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class BoardController : Controller
    {
        private readonly IBoardRepository _boardService;
        
        public BoardController(IBoardRepository boardService)
        {
            _boardService = boardService;
        }
        
        [HttpGet(ApiRoutes.Board.GetAll)]
        public IActionResult GetAllAsync()
        {
            var boards =  _boardService.GetBoards();
            
            var bordsResponse = new List<BoardResponse>();

            foreach (var board in boards)
            {
                bordsResponse.Add(new BoardResponse
                {
                    Id = board.Id,
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
        
        [HttpPost(ApiRoutes.Board.Create)]
        public IActionResult CreateAsync([FromBody] CreateBoardRequest boardRequest)
        {
            var newBoardId = Guid.NewGuid();

            var board = new Board
            {
                Id = newBoardId,
                Name = boardRequest.Name,
                Description = boardRequest.Description,
                UserId = Guid.NewGuid().ToString(),
            };

             _boardService.CreateBoard(board);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Board.Get.Replace("{boardId}", board.Id.ToString())}";

            var response = new BoardResponse 
            { 
                Id = board.Id,
                Name = board.Name,
                UserId = board.UserId,
                Description = board.Description,
                Cards = new List<CardResponse>()
            };

            return Created(location, response);
        }
    }
}