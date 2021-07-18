using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Contracts.V1.Responses;
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

        public async Task<List<Board>> GetBoardsAsNoTrackingAsync()
        {
            return await _dataContext.Boards.AsNoTracking().ToListAsync();
        }
        
        public async Task<Board> GetBoardByIdAsNoTrackingAsync(Guid boardId)
        {
            var a = await _dataContext.Boards
                .Include(m => m.Members)
                .Include(c => c.Cards)
                .ThenInclude(card => card.Tags)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == boardId);

            return a;
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

        public async Task<bool> DeleteBoardIdAsync(Board board)
        {
            _dataContext.Boards.Remove(board);
            var deleted = await _dataContext.SaveChangesAsync();
            
            return deleted > 0;
        }

        public async Task<Board> GetBoardOwnedByUserAsNoTrackingAsync(Guid boardId, string userId)
        {
            return await _dataContext.Boards
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == boardId && x.UserId == userId);
        }

        public async Task<Board> GetBoardWhereUserIsMemberAsNoTrackingAsync(Guid boardId)
        {
            return await _dataContext.Boards
                .Include(x => x.Members)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == boardId);
        }
    }
}