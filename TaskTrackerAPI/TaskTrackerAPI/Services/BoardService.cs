using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        #region Board service

         public async Task<List<Board>> GetBoardsAsync()
        {
            return await _dataContext.Boards.ToListAsync();
        }

        public async Task<bool> CreateBoardAsync(string email, Board board)
        {
            await _dataContext.Boards.AddAsync(board);
            await AddMemberToBoardAsync(email, board);
            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<Board> GetBoardByIdAsync(Guid boardId)
        {
            return await _dataContext.Boards
                .Include(x => x.Cards)
                .Include(x => x.Members)
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
            var board = await GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return false;
            }
            
            _dataContext.Cards.RemoveRange(board.Cards);
            _dataContext.Boards.Remove(board);
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

        public async Task<bool> UserBelongsToBoard(Guid boardId, string email)
        {
            var board = await _dataContext.Boards
                .AsNoTracking()
                .Include(x => x.Members)
                .SingleOrDefaultAsync(x => x.Id == boardId);

            if (board == null)
            {
                return false;
            }

            var result = board.Members.SingleOrDefault(x => x.MemberEmail == email);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Member service

        public async Task<List<Member>> GetMembersAsync()
        {
            return await _dataContext.Members.ToListAsync();
        }

        public async Task<Member> GetMemberAsync(string email)
        {
            return await _dataContext.Members
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> DeleteMemberFromBoardAsync(string email, Board board)
        {
            var memberToDelete = await GetMemberAsync(email);

            if (memberToDelete == null)
            {
                return true;
            }

            var boardMember = await _dataContext.BoardMembers
                .Where(x => x.MemberEmail == memberToDelete.Email && 
                            x.BoardId == board.Id).SingleOrDefaultAsync();

            _dataContext.BoardMembers.Remove(boardMember);
            var result = await _dataContext.SaveChangesAsync();
                
            return result > 0;
        }
        
        public async Task<bool> AddMemberToBoardAsync(string email, Board board)
        {
            var memberToAdd = await GetMemberAsync(email);

            if (memberToAdd == null)
            {
                return false;
            }

            if (board.Members.SingleOrDefault(x => x.MemberEmail == email) != null)
            {
                return true;
            }
            
            var boardMember = new BoardMember
            {
                Board = board,
                Member = memberToAdd,
                BoardId = board.Id,
                MemberEmail = memberToAdd.Email
            };

            await _dataContext.BoardMembers.AddAsync(boardMember);
            var result = await _dataContext.SaveChangesAsync();

            return result > 0;
        }

        #endregion
        
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
            var card = await GetCardByIdAsync(cardId);

            if (card == null)
            {
                return false;
            }

            _dataContext.Cards.Remove(card);
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
