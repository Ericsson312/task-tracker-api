using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1.Requiests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Extensions;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TasksToDoController : Controller
    {
        private readonly ITaskToDoService _taskService;
        public TasksToDoController(ITaskToDoService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet(ApiRoutes.Tasks.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _taskService.GetTasksAsync());
        }

        [HttpGet(ApiRoutes.Tasks.Get)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid taskId)
        {
            var taskToDo = await _taskService.GetTaskByIdAsync(taskId);

            if (taskToDo == null)
            {
                return NotFound();
            }

            return Ok(taskToDo);
        }

        [HttpPut(ApiRoutes.Tasks.Update)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid taskId, [FromBody] UpdateTaskToDoRequest taskRequest)
        {
            var userOwnsTaskToDo = await _taskService.UserOwnsTaskToDoAsync(taskId, HttpContext.GetUserId());

            if (!userOwnsTaskToDo)
            {
                return BadRequest(new { error = "You do not own this task" });
            }

            var taskToUpdate = await _taskService.GetTaskByIdAsync(taskId);
            taskToUpdate.Name = taskRequest.Name;

            var updated = await _taskService.UpdateTaskAsync(taskToUpdate);

            if (updated)
            {
                return Ok(taskToUpdate);
            }

            return NotFound();
        }

        [HttpPost(ApiRoutes.Tasks.Create)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTaskToDoRequest taskRequest)
        {
            var newPostId = Guid.NewGuid();

            var taskToDo = new TaskToDo
            {
                Id = newPostId,
                Name = taskRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = taskRequest.Tags.Select(tagName => new TaskToDoTag { TagName = tagName, TaskToDoId = newPostId }).ToList()
            };

            await _taskService.CreateTaskAsync(taskToDo);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Tasks.Get.Replace("{taskId}", taskToDo.Id.ToString())}";

            var response = new TaskToDoResponse { Id = taskToDo.Id };

            return Created(location, response);
        }

        [HttpDelete(ApiRoutes.Tasks.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid taskId)
        {
            var userOwnsTaskToDo = await _taskService.UserOwnsTaskToDoAsync(taskId, HttpContext.GetUserId());

            if (!userOwnsTaskToDo)
            {
                return BadRequest(new { error = "You do not own this task" });
            }

            var deleted = await _taskService.DeleteTaskAsync(taskId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
