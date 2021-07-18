using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface IBoardRepository
    {
        Task<Board> GetBoardByIdAsNoTrackingAsync(Guid boardId);
        Task<List<Board>> GetBoardsAsNoTrackingAsync();
        Task<bool> CreateBoardAsync(Board board);
        Task<bool> UpdateBoardAsync(Board board);
        Task<bool> DeleteBoardIdAsync(Board board);
        Task<Board> GetBoardOwnedByUserAsNoTrackingAsync(Guid boardId, string userId);
        Task<Board> GetBoardWhereUserIsMemberAsNoTrackingAsync(Guid boardId);
    }
}