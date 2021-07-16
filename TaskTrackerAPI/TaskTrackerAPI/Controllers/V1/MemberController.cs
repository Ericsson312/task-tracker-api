using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    public class MemberController : Controller
    {
        private readonly IBoardService _boardService;

        public MemberController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet(ApiRoutes.Members.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            var members = await _boardService.GetMembersAsync();
            var memberResponse = members.Select(x => new MemberResponse
            {
                Email = x.Email
            });

            return Ok(memberResponse);
        }
        
        [HttpGet(ApiRoutes.Members.Get)]
        public async Task<IActionResult> GetAsync([FromRoute] string email)
        {
            var member = await _boardService.GetMemberAsync(email);
            
            if (member == null)
            {
                return NotFound();
            }

            return Ok(new MemberResponse
            {
                Email = member.Email
            });
        }
        
        [HttpPost(ApiRoutes.Members.Create)]
        public async Task<IActionResult> CreateAsync([FromRoute] Guid boardId, [FromBody] CreateMemberRequest memberRequest)
        {
            var boardToUpdate = await _boardService.GetBoardByIdAsync(boardId);

            if (boardToUpdate == null)
            {
                return BadRequest(new { error = "The board does not exist" });
            }
            
            var deleted = await _boardService.AddMemberToBoardAsync(memberRequest.Email, boardToUpdate);
            
            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
        
        [HttpDelete(ApiRoutes.Members.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid boardId, [FromBody] DeleteMemberRequest memberRequest)
        {
            var boardToUpdate = await _boardService.GetBoardByIdAsync(boardId);

            if (boardToUpdate == null)
            {
                return BadRequest(new { error = "The board does not exist" });
            }

            if (!await _boardService.UserOwnsBoardAsync(boardId, HttpContext.GetUserId()))
            {
                return BadRequest(new { error = "Only board owners can exclude members" });
            }
            
            var deleted = await _boardService.DeleteMemberFromBoardAsync(memberRequest.Email, boardToUpdate);
            
            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}