using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Entities;
using SmartNameplate.Api.DTOs;

namespace SmartNameplate.Api.Services;

public class CardService : ICardService
{
    private readonly ApplicationDbContext _context;

    public CardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Card>> GetAllCardsAsync()
    {
        return await _context.Cards
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Card?> GetCardByIdAsync(int id)
    {
        return await _context.Cards.FindAsync(id);
    }

    public async Task<Card> CreateCardAsync(CreateCardDto cardDto)
    {
        var card = new Card
        {
            Name = cardDto.Name,
            Description = cardDto.Description,
            Status = cardDto.Status,
            ThumbnailA = cardDto.ThumbnailA,
            ThumbnailB = cardDto.ThumbnailB,
            ContentA = cardDto.ContentA,
            ContentB = cardDto.ContentB,
            IsSameBothSides = cardDto.IsSameBothSides,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();
        return card;
    }
    public async Task<Card?> UpdateCardAsync(int id, UpdateCardDto cardDto)
    {
        var card = await _context.Cards.FindAsync(id);
        if (card == null)
            return null;

        if (!string.IsNullOrEmpty(cardDto.Name))
            card.Name = cardDto.Name;
        
        if (cardDto.Description != null)
            card.Description = cardDto.Description;
        
        if (cardDto.Status.HasValue)
            card.Status = cardDto.Status.Value;
        
        if (cardDto.ThumbnailA != null)
            card.ThumbnailA = cardDto.ThumbnailA;
        
        if (cardDto.ThumbnailB != null)
            card.ThumbnailB = cardDto.ThumbnailB;
        
        if (cardDto.ContentA.HasValue)
            card.ContentA = cardDto.ContentA;
            
        if (cardDto.ContentB.HasValue)
            card.ContentB = cardDto.ContentB;

        if (cardDto.IsSameBothSides.HasValue)
            card.IsSameBothSides = cardDto.IsSameBothSides.Value;

        card.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return card;
    }

    public async Task<bool> DeleteCardAsync(int id)
    {
        Console.WriteLine($"[CardService.DeleteCardAsync] 開始刪除卡片，ID: {id}");
        
        var card = await _context.Cards.FindAsync(id);
        if (card == null)
        {
            Console.WriteLine($"[CardService.DeleteCardAsync] 卡片不存在，ID: {id}");
            return false;
        }

        Console.WriteLine($"[CardService.DeleteCardAsync] 找到卡片：{card.Name}，準備刪除");
        
        _context.Cards.Remove(card);
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"[CardService.DeleteCardAsync] 卡片刪除完成，ID: {id}");
        return true;
    }

    public async Task<IEnumerable<Card>> GetCardsByStatusAsync(CardStatus status)
    {
        return await _context.Cards
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }
}