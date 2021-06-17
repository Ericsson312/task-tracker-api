using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Contracts;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Controllers
{
    public class TasksController : Controller
    {
        private List<TaskToDo> _tasks;

        public TasksController()
        {
            _tasks = new List<TaskToDo>();
            for (int i = 0; i < 5; i++)
            {
                _tasks.Add(new TaskToDo { Id = Guid.NewGuid().ToString() });
            }
        }

        [HttpGet(ApiRoutes.Tasks.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_tasks);
        }
    }
}
