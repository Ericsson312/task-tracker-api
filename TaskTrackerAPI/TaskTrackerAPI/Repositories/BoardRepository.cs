using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly DataContext _dataContext;

        public BoardRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Board>> GetBoardsAsync(PaginationFilter paginationFilter)
        {
            // calculate the amount of pages that needs to be skipped
            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
            
            return await _dataContext.Boards
                .Include(m => m.Members)
                .Include(c => c.Cards)
                .ThenInclude(card => card.Tags)
                .Skip(skip)
                .Take(paginationFilter.PageSize)
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<Board> GetBoardByIdAsync(Guid boardId)
        {
            return await _dataContext.Boards
                .Include(m => m.Members)
                .Include(c => c.Cards)
                .ThenInclude(card => card.Tags)
                .SingleOrDefaultAsync(x => x.Id == boardId);
        }
        
        public async Task<Board> GetBoardByCardIdAsync(Guid cardId)
        {
            return await _dataContext.Boards.AsNoTracking()
                .Include(c => c.Cards)
                .FirstOrDefaultAsync(x => x.Cards.Exists(xx => xx.Id == cardId));
        }

        public async Task<bool> CreateBoardAsync(Board board)
        {
            await _dataContext.Boards.AddAsync(board);
            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdateBoardAsync(Board board)
        {
            _dataContext.Boards.Update(board);
            var updated = await _dataContext.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeleteBoardAsync(Board board)
        {
            _dataContext.Boards.Remove(board);
            var deleted = await _dataContext.SaveChangesAsync();
            
            return deleted > 0;
        }

        public async Task<Board> GetBoardOwnedByUserAsync(Guid boardId, string userId)
        {
            return await _dataContext.Boards
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == boardId && x.UserId == userId);
        }

        public async Task<Board> GetBoardWhereUserIsMemberAsync(Guid boardId)
        {
            return await _dataContext.Boards
                .Include(x => x.Members)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == boardId);
        }
    }
}