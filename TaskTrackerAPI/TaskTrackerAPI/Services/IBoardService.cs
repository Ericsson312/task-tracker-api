using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public interface IBoardService
    {
        #region Board service
        Task<List<Board>> GetBoardsAsync();
        Task<bool> CreateBoardAsync(Board board);
        Task<Board> GetBoardByIdAsync(Guid boardId);
        Task<bool> UpdateBoardAsync(Board board);
        Task<bool> DeleteBoardIdAsync(Guid cardId);
        Task<bool> UserOwnsBoardAsync(Guid boardId, string userId);
        #endregion
        
        #region Card service
        Task<List<Card>> GetCardsAsync();
        Task<Card> GetCardByIdAsync(Guid cardId);
        Task<bool> CreateCardAsync(Card card);
        Task<bool> UpdateCardAsync(Card card);
        Task<bool> DeleteCardAsync(Guid cardId);
        Task<bool> UserOwnsCardAsync(Guid cardId, string userId);
        #endregion

        #region Tag service
        Task<List<Tag>> GetTagsAsync();
        Task<Tag> GetTagByNameAsync(string tagName);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(string tagName);
        #endregion
    }
}
