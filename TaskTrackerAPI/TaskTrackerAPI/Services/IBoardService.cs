using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public interface IBoardService
    {
        #region Boear service
        Task<List<Board>> GetBoardsAsync();
        Task<bool> CreateBoardAsync(Board board);
        #endregion
        
        #region Card service
        Task<List<Card>> GetCardsAsync();
        Task<Card> GetCardByIdAsync(Guid taskId);
        Task<bool> CreateCardAsync(Card taskToDo);
        Task<bool> UpdateCardAsync(Card taskToUpdate);
        Task<bool> DeleteCardAsync(Guid taskId);
        Task<bool> UserOwnsCardAsync(Guid taskId, string userId);
        #endregion

        #region Tag service
        Task<List<Tag>> GetTagsAsync();
        Task<Tag> GetTagByNameAsync(string tagName);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(string tagName);
        #endregion
    }
}
