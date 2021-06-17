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
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet(ApiRoutes.Tasks.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_taskService.GetTasks());
        }

        [HttpGet(ApiRoutes.Tasks.Get)]
        public IActionResult Get([FromRoute] Guid taskId)
        {
            var taskToDo = _taskService.GetTaskById(taskId);

            if (taskToDo == null)
            {
                return NotFound();
            }

            return Ok(taskToDo);
        }

        [HttpPut(ApiRoutes.Tasks.Update)]
        public IActionResult Update([FromRoute] Guid taskId, [FromBody] UpdateTaskRequest taskRequest)
        {
            var taskToUpdate = new TaskToDo()
            {
                Id = taskId,
                TaskName = taskRequest.TaskName
            };

            var updated = _taskService.UpdateTask(taskToUpdate);

            if (updated)
            {
                return Ok(taskToUpdate);
            }

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Tasks.Delete)]
        public IActionResult Delete([FromRoute] Guid taskId)
        {
            var deleted = _taskService.DeleteTask(taskId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPost(ApiRoutes.Tasks.Create)]
        public IActionResult Create([FromBody] CreateTaskRequest taskRequest)
        {
            var taskToDo = new TaskToDo { Id = taskRequest.Id, TaskName = taskRequest.TaskName };

            if (taskToDo.Id == Guid.Empty)
            {
                taskToDo.Id = Guid.NewGuid();
            }

            _taskService.GetTasks().Add(taskToDo);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = $"{baseUrl}/{ApiRoutes.Tasks.Get.Replace("{taskId}", taskToDo.Id.ToString())}";

            var response = new TaskResponse { Id = taskToDo.Id };

            return Created(location, response);
        }
    }
}
