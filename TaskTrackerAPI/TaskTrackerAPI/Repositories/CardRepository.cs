using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly DataContext _dataContext;

        public CardRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<List<Card>> GetCardsAsync()
        {
            return await _dataContext.Cards.Include(x => x.Tags).AsNoTracking().ToListAsync();
        }

        public async Task<Card> GetCardByIdAsync(Guid cardId)
        {
            return await _dataContext.Cards
                .Include(x => x.Tags)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == cardId);
        }

        public async Task<bool> CreateCardAsync(Card card)
        {
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

        public async Task<bool> DeleteCardAsync(Card card)
        {
            _dataContext.Cards.Remove(card);
            var deleted = await _dataContext.SaveChangesAsync();

            return deleted > 0;
        }
    }
}