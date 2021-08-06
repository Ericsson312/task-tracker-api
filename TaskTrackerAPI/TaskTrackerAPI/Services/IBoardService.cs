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
        Task<bool> DeleteBoardByIdAsync(Guid cardId);
        Task<bool> UserOwnsBoardAsync(Guid boardId, string userId);
        Task<bool> UserBelongsToBoard(Guid boardId, string email);
        
        #endregion
        
        #region Card service
        
        Task<List<Card>> GetCardsAsync();
        Task<Card> GetCardByIdAsync(Guid cardId);
        Task<bool> CreateCardAsync(Card card);
        Task<bool> UpdateCardAsync(Card card);
        Task<bool> DeleteCardAsync(Guid cardId);
        
        #endregion

        #region Tag service
        
        Task<List<Tag>> GetTagsAsync();
        Task<Tag> GetTagByNameAsync(string tagName);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(string tagName);
        
        #endregion

        #region Member service

        Task<List<Member>> GetMembersAsync();
        Task<Member> GetMemberAsync(string email);
        Task<bool> DeleteMemberFromBoardAsync(string email, Board board);
        Task<bool> AddMemberToBoardAsync(string email, Board board);

        #endregion
    }
}
