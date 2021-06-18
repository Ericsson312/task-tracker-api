using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Contracts.V1.Requiests;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers
{
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
            var taskToUpdate = new TaskToDo()
            {
                Id = taskId,
                Name = taskRequest.Name
            };

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
            var taskToDo = new TaskToDo { Name = taskRequest.Name };

            await _taskService.CreateTaskAsync(taskToDo);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Tasks.Get.Replace("{taskId}", taskToDo.Id.ToString())}";

            var response = new TaskToDoResponse { Id = taskToDo.Id };

            return Created(location, response);
        }

        [HttpDelete(ApiRoutes.Tasks.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid taskId)
        {
            var deleted = await _taskService.DeleteTaskAsync(taskId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
