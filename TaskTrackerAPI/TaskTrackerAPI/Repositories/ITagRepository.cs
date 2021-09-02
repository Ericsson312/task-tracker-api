using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsAsync(PaginationFilter paginationFilter);
        Task<Tag> GetTagByNameAsync(string tagName);
        Task<bool> AddTagAsync(Tag tag);
        Task<bool> RemoveTagAsync(Tag tag);
    }
}