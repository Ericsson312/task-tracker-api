using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsAsync();
        Task<Tag> GetTagByNameAsNoTrackingAsync(string tagName);
        Task<bool> AddTagAsync(Tag tag);
        Task<bool> RemoveRangeCardTagsAsync(string tagName);
        Task<bool> RemoveTagAsync(Tag tag);
    }
}