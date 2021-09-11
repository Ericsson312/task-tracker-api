using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Cache;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Contracts.V1.Requests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Examples.V1.Requests.Queries;
using TaskTrackerApi.Examples.V1.Responses;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Helpers;
using TaskTrackerApi.Repositories;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "MustWorkForTasker")]
    [Produces("application/json")]
    public class TagsController : Controller
    {
        private readonly IBoardService _taskService;
        private readonly IUriService _uriService;
        
        public TagsController(IBoardService taskService, IUriService uriService)
        {
            _taskService = taskService;
            _uriService = uriService;
        }

        /// <summary>
        /// Returns all the tags in the system
        /// </summary>
        /// <response code="200">Returns all the tags in the system</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        [ProducesResponseType(typeof(TagResponse), 200)]
        [Cached(600)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationQuery paginationQuery)
        {
            var pageNumber = paginationQuery.PageNumber;
            var pageSize = paginationQuery.PageSize;
            
            var filter = new PaginationFilter
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            var tags = await _taskService.GetTagsAsync(filter);
            
            var tagResponses = tags.Select(x => new TagResponse
            {
                Name = x.Name
            }).ToList();
            
            var paginationResponse = PaginationHelpers.CreatePaginationResponse(_uriService, filter, tagResponses);
            
            // var pagedResponse = new PagedResponse<TagResponse>
            // {
            //     Data = tagResponses,
            //     PageNumber = pageNumber,
            //     PageSize = pageSize,
            //     ElementsCont = tags.Count
            // };

            return Ok(paginationResponse);
        }

        /// <summary>
        /// Returns a certain tag in the system
        /// </summary>
        /// <response code="200">Returns a certain tag in the system</response>
        /// <response code="404">Unable to get tag by passed name</response>
        [HttpGet(ApiRoutes.Tags.Get)]
        [ProducesResponseType(typeof(TagResponse), 200)]
        [Cached(600)]
        public async Task<IActionResult> GetAsync([FromRoute] string tagName)
        {
            var tag = await _taskService.GetTagByNameAsync(tagName);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(new Response<TagResponse>(new TagResponse
            {
                Name = tag.Name
            }));
        }

        /// <summary>
        /// Creates a tag in the system
        /// </summary>
        /// <response code="201">Creates a tag in the system</response>
        ///<response code="400">Unable to create tag due to validation error</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTagRequest request)
        {
            var newTag = new Tag 
            {
                Name = request.Name,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            var create = await _taskService.CreateTagAsync(newTag);

            if (!create)
            {
                return BadRequest(new ErrorResponse{ Errors = new List<ErrorModel>{ new ErrorModel{ Message = "Unable to create tag" } }});
            }

            // var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            // var location = $"{baseUrl}/{ApiRoutes.Tags.Get.Replace("{tagName}", newTag.Name)}";
            
            var locationUri = _uriService.GetTagUri(newTag.Name);

            var response = new TagResponse { Name = newTag.Name };

            return Created(locationUri, new Response<TagResponse>(response));
        }

        /// <summary>
        /// Deletes a tag in the system
        /// </summary>
        /// <response code="204">Deletes a tag from the system</response>
        /// <response code="404">Unable to remove tag due to validation error</response>
        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string tagName)
        {
            var deleted = await _taskService.DeleteTagAsync(tagName);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
