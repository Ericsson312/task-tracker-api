using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Services
{
    public interface ICardService
    {
        Task<List<Card>> GetCardsAsync();
        Task<Card> GetCardByIdAsync(Guid taskId);
        Task<bool> CreateCardAsync(Card taskToDo);
        Task<bool> UpdateCardAsync(Card taskToUpdate);
        Task<bool> DeleteCardAsync(Guid taskId);
        Task<bool> UserOwnsCardAsync(Guid taskId, string UserId);

        Task<List<Tag>> GetTagsAsync();
        Task<Tag> GetTagByNameAsync(string tagName);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(string tagName);
    }
}
