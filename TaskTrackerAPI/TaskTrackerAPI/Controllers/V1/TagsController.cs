﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "MustWorkForTasker")]
    public class TagsController : Controller
    {
        private readonly IBoardService _taskService;
        public TagsController(IBoardService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            var tags = await _taskService.GetTagsAsync();
            var tagResponses = tags.Select(x => new TagResponse
            {
                Name = x.Name
            });

            return Ok(tagResponses);
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> GetAsync([FromRoute] string tagName)
        {
            var tag = await _taskService.GetTagByNameAsync(tagName);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(new TagResponse { Name = tag.Name });
        }

        [HttpPost(ApiRoutes.Tags.Create)]
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
                return BadRequest(new { error = "Unable to create tag" });
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Tags.Get.Replace("{tagName}", newTag.Name)}";

            var response = new TagResponse { Name = newTag.Name };

            return Created(location, response);
        }

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
