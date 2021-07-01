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
            return await _dataContext.TasksToDo
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == taskId);
        }

        public async Task<List<TaskToDo>> GetTasksAsync()
        {
            return await _dataContext.TasksToDo.Include(x => x.Tags).ToListAsync();
        }

        public async Task<bool> CreateTaskAsync(TaskToDo taskToDo)
        {
            taskToDo.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());

            await AddNewTag(taskToDo);

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
            var taskToDo = await _dataContext.TasksToDo
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == taskId && x.UserId == UserId);

            if (taskToDo == null)
            {
                return false;
            }

            return true;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _dataContext.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await _dataContext.Tags
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == tagName);
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            tag.Name = tag.Name.ToLower();

            var tagExist = await _dataContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tag.Name);

            if (tagExist != null)
            {
                return true;
            }

            await _dataContext.Tags.AddAsync(new Tag
            {
                Name = tag.Name,
                CreatedOn = DateTime.UtcNow,
                CreatorId = tag.CreatorId
            });

            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await _dataContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tagName.ToLower());

            if (tag == null)
            {
                return true;
            }

            var taskToDoTag = await _dataContext.TaskToDoTags.Where(x => x.TagName == tagName.ToLower()).ToListAsync();

            _dataContext.TaskToDoTags.RemoveRange(taskToDoTag);
            _dataContext.Tags.Remove(tag);
            var result = await _dataContext.SaveChangesAsync();

            return result > taskToDoTag.Count;
        }

        private async Task AddNewTag(TaskToDo taskToDo)
        {
            foreach (var tag in taskToDo.Tags)
            {
                var tagExist = await _dataContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tag.TagName);

                if (tagExist != null)
                {
                    continue;
                }

                await _dataContext.Tags.AddAsync(new Tag 
                { 
                    Name = tag.TagName, 
                    CreatedOn = DateTime.UtcNow, 
                    CreatorId = taskToDo.UserId 
                });
            }
        }
    }
}
