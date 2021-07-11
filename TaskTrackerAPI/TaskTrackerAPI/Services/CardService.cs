using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public class CardService : ICardService
    {
        private readonly DataContext _dataContext;

        public CardService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Card> GetCardByIdAsync(Guid cardId)
        {
            return await _dataContext.TasksToDo
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == cardId);
        }

        public async Task<List<Card>> GetCardsAsync()
        {
            return await _dataContext.TasksToDo.Include(x => x.Tags).ToListAsync();
        }

        public async Task<bool> CreateCardAsync(Card card)
        {
            card.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());

            await AddNewTag(card);

            await _dataContext.TasksToDo.AddAsync(card);
            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateCardAsync(Card cardToUpdate)
        {
            _dataContext.TasksToDo.Update(cardToUpdate);
            var updated = await _dataContext.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeleteCardAsync(Guid cardId)
        {
            var cardToDelete = await GetCardByIdAsync(cardId);

            if (cardToDelete == null)
            {
                return false;
            }

            _dataContext.TasksToDo.Remove(cardToDelete);
            var deleted = await _dataContext.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsCardAsync(Guid cardId, string UserId)
        {
            var card = await _dataContext.TasksToDo
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == cardId && x.UserId == UserId);

            if (card == null)
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

        private async Task AddNewTag(Card card)
        {
            foreach (var tag in card.Tags)
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
                    CreatorId = card.UserId 
                });
            }
        }
    }
}
