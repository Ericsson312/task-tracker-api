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
using TaskTrackerApi.Examples.V1.Requests.Queries;
using TaskTrackerApi.Examples.V1.Responses;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class MemberController : Controller
    {
        private readonly IBoardService _boardService;

        public MemberController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        /// <summary>
        /// Returns all the members in the system
        /// </summary>
        /// <response code="200">Returns all the members in the system</response>
        [HttpGet(ApiRoutes.Members.GetAll)]
        [ProducesResponseType(typeof(MemberResponse), 200)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationQuery paginationQuery)
        {
            var pageNumber = paginationQuery.PageNumber;
            var pageSize = paginationQuery.PageSize;
            
            var filter = new PaginationFilter
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var members = await _boardService.GetMembersAsync(filter);
            
            var memberResponse = members.Select(x => new MemberResponse
            {
                Email = x.Email
            }).ToList();
            
            var pagedResponse = new PagedResponse<MemberResponse>
            {
                Data = memberResponse,
                PageNumber = pageNumber,
                PageSize = pageSize,
                ElementsCont = members.Count
            };

            return Ok(pagedResponse);
        }
        
        /// <summary>
        /// Returns a certain member in the system
        /// </summary>
        /// <response code="200">Returns a certain member in the system</response>
        /// <response code="404">Unable to find member</response>
        [HttpGet(ApiRoutes.Members.Get)]
        [ProducesResponseType(typeof(MemberResponse), 200)]
        public async Task<IActionResult> GetAsync([FromRoute] string email)
        {
            var member = await _boardService.GetMemberAsync(email);
            
            if (member == null)
            {
                return NotFound();
            }

            return Ok(new Response<MemberResponse>(new MemberResponse
            {
                Email = member.Email
            }));
        }
        
        /// <summary>
        /// Creates new member for the board in the system
        /// </summary>
        /// <response code="204">Creates new member for the board in the system</response>
        /// <response code="400">Unable to create member due to validation error</response>
        /// <response code="404">Unable to create member</response>
        [HttpPost(ApiRoutes.Members.Create)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> CreateAsync([FromRoute] Guid boardId, [FromBody] CreateMemberRequest memberRequest)
        {
            var boardToUpdate = await _boardService.GetBoardByIdAsync(boardId);

            if (boardToUpdate == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to create member due to validation error" } }
                });
            }
            
            var created = await _boardService.AddMemberToBoardAsync(memberRequest.Email, boardToUpdate);
            
            if (created)
            {
                return NoContent();
            }

            return NotFound();
        }
        
        /// <summary>
        /// Deletes a member from board in the system
        /// </summary>
        /// <response code="204">Deletes a member from board in the system</response>
        /// <response code="404">Unable to delete member from board</response>
        /// <response code="400">Unable to delete member due to validation error</response>
        [HttpDelete(ApiRoutes.Members.Delete)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid boardId, [FromBody] DeleteMemberRequest memberRequest)
        {
            var boardToUpdate = await _boardService.GetBoardByIdAsync(boardId);

            if (boardToUpdate == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to delete member from board" } }
                });
            }

            if (!await _boardService.UserOwnsBoardAsync(boardId, HttpContext.GetUserId()))
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to delete member due to validation error" } }
                });
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