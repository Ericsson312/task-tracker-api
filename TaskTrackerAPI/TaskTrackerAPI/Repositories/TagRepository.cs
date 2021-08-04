using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _dataContext;

        public TagRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
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

        public async Task<bool> AddTagAsync(Tag tag)
        {
            await _dataContext.Tags.AddAsync(tag);
            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> RemoveRangeCardTagsAsync(string tagName)
        {
            var cardTags = await _dataContext.CardTags.Where(x => x.TagName == tagName.ToLower()).ToListAsync();
            _dataContext.CardTags.RemoveRange(cardTags);
            var result = await _dataContext.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> RemoveTagAsync(Tag tag)
        {
            _dataContext.Tags.Remove(tag);
            var result = await _dataContext.SaveChangesAsync();

            return result > 0;
        }
    }
}