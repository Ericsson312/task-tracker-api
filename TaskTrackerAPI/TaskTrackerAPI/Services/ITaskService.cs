using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public interface ITaskService
    {
        List<TaskToDo> GetTasks();
        TaskToDo GetTaskById(Guid taskId);
        bool UpdateTask(TaskToDo taskToUpdate);
        bool DeleteTask(Guid taskId);
    }
}
