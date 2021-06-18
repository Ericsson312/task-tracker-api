using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public class TaskToDoService : ITaskToDoService
    {
        private readonly DataContext _dataContext;

        public TaskToDoService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<TaskToDo> GetTaskByIdAsync(Guid taskId)
        {
            return await _dataContext.TasksToDo.SingleOrDefaultAsync(x => x.Id == taskId);
        }

        public async Task<List<TaskToDo>> GetTasksAsync()
        {
            return await _dataContext.TasksToDo.ToListAsync();
        }

        public async Task<bool> CreateTaskAsync(TaskToDo taskToDo)
        {
            await _dataContext.TasksToDo.AddAsync(taskToDo);
            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateTaskAsync(TaskToDo taskToUpdate)
        {
            _dataContext.TasksToDo.Update(taskToUpdate);
            var updated = await _dataContext.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            var taskToDelete = await GetTaskByIdAsync(taskId);

            if (taskToDelete == null)
            {
                return false;
            }

            _dataContext.TasksToDo.Remove(taskToDelete);
            var deleted = await _dataContext.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsTaskToDoAsync(Guid taskId, string UserId)
        {
            var taskToDo = await _dataContext.TasksToDo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == taskId && x.UserId == UserId);

            if (taskToDo == null)
            {
                return false;
            }

            return true;
        }
    }
}
