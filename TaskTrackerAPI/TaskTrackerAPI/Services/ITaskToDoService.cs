using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public interface ITaskToDoService
    {
        Task<List<TaskToDo>> GetTasksAsync();
        Task<TaskToDo> GetTaskByIdAsync(Guid taskId);
        Task<bool> CreateTaskAsync(TaskToDo taskToDo);
        Task<bool> UpdateTaskAsync(TaskToDo taskToUpdate);
        Task<bool> DeleteTaskAsync(Guid taskId);
        Task<bool> UserOwnsTaskToDoAsync(Guid taskId, string UserId);
    }
}
