using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface IBoardRepository
    {
        Task<Board> GetBoardByIdAsync(Guid boardId);
        Task<Board> GetBoardByCardIdAsync(Guid cardId);
        Task<List<Board>> GetBoardsAsync();
        Task<bool> CreateBoardAsync(Board board);
        Task<bool> UpdateBoardAsync(Board board);
        Task<bool> DeleteBoardAsync(Board board);
        Task<Board> GetBoardOwnedByUserAsync(Guid boardId, string userId);
        Task<Board> GetBoardWhereUserIsMemberAsync(Guid boardId);
    }
}