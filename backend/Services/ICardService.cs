using SmartNameplate.Api.Entities;
using SmartNameplate.Api.DTOs;

namespace SmartNameplate.Api.Services;

public interface ICardService
{
    Task<IEnumerable<Card>> GetAllCardsAsync();
    Task<Card?> GetCardByIdAsync(int id);
    Task<Card> CreateCardAsync(CreateCardDto cardDto);
    Task<Card?> UpdateCardAsync(int id, UpdateCardDto cardDto);
    Task<bool> DeleteCardAsync(int id);
    Task<IEnumerable<Card>> GetCardsByStatusAsync(CardStatus status);
}