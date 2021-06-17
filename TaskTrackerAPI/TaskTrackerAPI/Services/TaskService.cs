using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly List<TaskToDo> _tasks;

        public TaskService()
        {
            _tasks = new List<TaskToDo>();
            for (int i = 0; i < 5; i++)
            {
                _tasks.Add(new TaskToDo
                {
                    Id = Guid.NewGuid(),
                    TaskName = "Task number " + i
                });
            }
        }

        public bool DeleteTask(Guid taskId)
        {
            var taskToDelete = GetTaskById(taskId);

            if (taskToDelete == null)
            {
                return false;
            }

            _tasks.Remove(taskToDelete);

            return true;
        }

        public TaskToDo GetTaskById(Guid taskId)
        {
            return  _tasks.SingleOrDefault(x => x.Id == taskId);
        }

        public List<TaskToDo> GetTasks()
        {
            return _tasks;
        }

        public bool UpdateTask(TaskToDo taskToUpdate)
        {
            var taskExists = GetTaskById(taskToUpdate.Id) != null;

            if (!taskExists)
            {
                return false;
            }

            var index = _tasks.FindIndex(x => x.Id == taskToUpdate.Id);
            _tasks[index] = taskToUpdate;

            return true;
        }
    }
}
