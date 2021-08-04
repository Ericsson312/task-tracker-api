using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface ICardRepository
    {
        Task<List<Card>> GetCardsAsync();
        Task<Card> GetCardByIdAsync(Guid cardId);
        Task<bool> CreateCardAsync(Card card);
        Task<bool> UpdateCardAsync(Card card);
        Task<bool> DeleteCardAsync(Card card);
    }
}