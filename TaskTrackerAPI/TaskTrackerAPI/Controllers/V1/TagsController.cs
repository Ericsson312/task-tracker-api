using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class TagsController : Controller
    {
        private readonly ITaskToDoService _taskService;
        public TagsController(ITaskToDoService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet(ApiRoutes.Tags.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _taskService.GetTagsAsync());
        }
    }
}
