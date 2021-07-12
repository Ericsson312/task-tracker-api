using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public class BoardService : IBoardService
    {
        private readonly DataContext _dataContext;

        public BoardService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<List<Board>> GetBoardsAsync()
        {
            return await _dataContext.Boards.ToListAsync();
        }

        public async Task<bool> CreateBoardAsync(Board board)
        {
            await _dataContext.Boards.AddAsync(board);
            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<Board> GetBoardByIdAsync(Guid boardId)
        {
            return await _dataContext.Boards
                .Include(x => x.Cards)
                .SingleOrDefaultAsync(x => x.Id == boardId);
        }

        public async Task<bool> UpdateBoardAsync(Board board)
        {
            _dataContext.Boards.Update(board);
            var updated = await _dataContext.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeleteBoardIdAsync(Guid boardId)
        {
            var boardToDelete = await GetBoardByIdAsync(boardId);

            if (boardToDelete == null)
            {
                return false;
            }
            
            _dataContext.Cards.RemoveRange(boardToDelete.Cards);
            _dataContext.Boards.Remove(boardToDelete);
            var deleted = await _dataContext.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsBoardAsync(Guid boardId, string userId)
        {
            var board = await _dataContext.Boards
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == boardId && x.UserId == userId);

            if (board == null)
            {
                return false;
            }

            return true;
        }

        #region Card service
        public async Task<List<Card>> GetCardsAsync()
        {
            return await _dataContext.Cards.Include(x => x.Tags).ToListAsync();
        }

        public async Task<Card> GetCardByIdAsync(Guid cardId)
        {
            return await _dataContext.Cards
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == cardId);
        }
        
        public async Task<bool> CreateCardAsync(Card card)
        {
            // normalize Tag names of the Card
            card.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());

            await AddNewTag(card);

            await _dataContext.Cards.AddAsync(card);
            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateCardAsync(Card card)
        {
            _dataContext.Cards.Update(card);
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

            _dataContext.Cards.Remove(cardToDelete);
            var deleted = await _dataContext.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsCardAsync(Guid cardId, string userId)
        {
            var card = await _dataContext.Cards
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == cardId && x.UserId == userId);

            if (card == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Tag service
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

            var cardTags = await _dataContext.CardTags.Where(x => x.TagName == tagName.ToLower()).ToListAsync();

            _dataContext.CardTags.RemoveRange(cardTags);
            _dataContext.Tags.Remove(tag);
            var result = await _dataContext.SaveChangesAsync();

            return result > cardTags.Count;
        }
        #endregion

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
