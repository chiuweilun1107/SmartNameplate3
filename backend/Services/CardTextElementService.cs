using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Services
{
    public interface ICardTextElementService
    {
        Task<List<CardTextElement>> GetCardTextElementsAsync(int cardId, string side);
        Task<CardTextElement?> GetCardTextElementAsync(int cardId, string side, string elementId);
        Task<CardTextElement> CreateCardTextElementAsync(CardTextElement element);
        Task<CardTextElement?> UpdateCardTextElementAsync(int id, CardTextElement element);
        Task<bool> DeleteCardTextElementAsync(int id);
        Task<List<CardInstanceData>> GetCardInstanceDataAsync(int cardId, string instanceName);
        Task<CardInstanceData> SaveCardInstanceDataAsync(CardInstanceData instanceData);
        Task<bool> DeleteCardInstanceAsync(int cardId, string instanceName);
    }

    public class CardTextElementService : ICardTextElementService
    {
        private readonly ApplicationDbContext _context;

        public CardTextElementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CardTextElement>> GetCardTextElementsAsync(int cardId, string side)
        {
            return await _context.CardTextElements
                .Where(e => e.CardId == cardId && e.Side == side)
                .OrderBy(e => e.SortOrder)
                .ToListAsync();
        }

        public async Task<CardTextElement?> GetCardTextElementAsync(int cardId, string side, string elementId)
        {
            return await _context.CardTextElements
                .FirstOrDefaultAsync(e => e.CardId == cardId && e.Side == side && e.ElementId == elementId);
        }

        public async Task<CardTextElement> CreateCardTextElementAsync(CardTextElement element)
        {
            element.CreatedAt = DateTime.UtcNow;
            element.UpdatedAt = DateTime.UtcNow;
            
            _context.CardTextElements.Add(element);
            await _context.SaveChangesAsync();
            
            return element;
        }

        public async Task<CardTextElement?> UpdateCardTextElementAsync(int id, CardTextElement element)
        {
            var existingElement = await _context.CardTextElements.FindAsync(id);
            if (existingElement == null)
                return null;

            existingElement.TagType = element.TagType;
            existingElement.TagLabel = element.TagLabel;
            existingElement.DefaultContent = element.DefaultContent;
            existingElement.IsRequired = element.IsRequired;
            existingElement.SortOrder = element.SortOrder;
            existingElement.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingElement;
        }

        public async Task<bool> DeleteCardTextElementAsync(int id)
        {
            var element = await _context.CardTextElements.FindAsync(id);
            if (element == null)
                return false;

            _context.CardTextElements.Remove(element);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CardInstanceData>> GetCardInstanceDataAsync(int cardId, string instanceName)
        {
            return await _context.CardInstanceDatas
                .Where(d => d.CardId == cardId && d.InstanceName == instanceName)
                .ToListAsync();
        }

        public async Task<CardInstanceData> SaveCardInstanceDataAsync(CardInstanceData instanceData)
        {
            var existingData = await _context.CardInstanceDatas
                .FirstOrDefaultAsync(d => 
                    d.CardId == instanceData.CardId && 
                    d.InstanceName == instanceData.InstanceName && 
                    d.Side == instanceData.Side && 
                    d.TagType == instanceData.TagType);

            if (existingData != null)
            {
                existingData.ContentValue = instanceData.ContentValue;
                existingData.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return existingData;
            }
            else
            {
                instanceData.CreatedAt = DateTime.UtcNow;
                instanceData.UpdatedAt = DateTime.UtcNow;
                _context.CardInstanceDatas.Add(instanceData);
                await _context.SaveChangesAsync();
                return instanceData;
            }
        }

        public async Task<bool> DeleteCardInstanceAsync(int cardId, string instanceName)
        {
            var instanceData = await _context.CardInstanceDatas
                .Where(d => d.CardId == cardId && d.InstanceName == instanceName)
                .ToListAsync();

            if (!instanceData.Any())
                return false;

            _context.CardInstanceDatas.RemoveRange(instanceData);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 