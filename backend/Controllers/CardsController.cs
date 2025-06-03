using Microsoft.AspNetCore.Mvc;
using SmartNameplate.Api.Services;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    /// <summary>
    /// 獲取所有桌牌
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardResponseDto>>> GetCards([FromQuery] CardStatus? status = null)
    {
        try
        {
            var cards = status.HasValue 
                ? await _cardService.GetCardsByStatusAsync(status.Value)
                : await _cardService.GetAllCardsAsync();

            var response = cards.Select(c => new CardResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Status = c.Status,
                ThumbnailA = c.ThumbnailA,
                ThumbnailB = c.ThumbnailB,
                ContentA = c.ContentA,
                ContentB = c.ContentB,
                IsSameBothSides = c.IsSameBothSides,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"獲取桌牌時發生錯誤: {ex.Message}");
        }
    }
    /// <summary>
    /// 根據 ID 獲取桌牌
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CardResponseDto>> GetCard(int id)
    {
        try
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null)
                return NotFound($"找不到 ID 為 {id} 的桌牌");

            var response = new CardResponseDto
            {
                Id = card.Id,
                Name = card.Name,
                Description = card.Description,
                Status = card.Status,
                ThumbnailA = card.ThumbnailA,
                ThumbnailB = card.ThumbnailB,
                ContentA = card.ContentA,
                ContentB = card.ContentB,
                IsSameBothSides = card.IsSameBothSides,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"獲取桌牌時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 創建新桌牌
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CardResponseDto>> CreateCard(CreateCardDto cardDto)
    {
        try
        {
            var card = await _cardService.CreateCardAsync(cardDto);
            var response = new CardResponseDto
            {
                Id = card.Id,
                Name = card.Name,
                Description = card.Description,
                Status = card.Status,
                ThumbnailA = card.ThumbnailA,
                ThumbnailB = card.ThumbnailB,
                ContentA = card.ContentA,
                ContentB = card.ContentB,
                IsSameBothSides = card.IsSameBothSides,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt
            };

            return CreatedAtAction(nameof(GetCard), new { id = card.Id }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"創建桌牌時發生錯誤: {ex.Message}");
        }
    }
    /// <summary>
    /// 更新桌牌
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CardResponseDto>> UpdateCard(int id, UpdateCardDto cardDto)
    {
        try
        {
            var card = await _cardService.UpdateCardAsync(id, cardDto);
            if (card == null)
                return NotFound($"找不到 ID 為 {id} 的桌牌");

            var response = new CardResponseDto
            {
                Id = card.Id,
                Name = card.Name,
                Description = card.Description,
                Status = card.Status,
                ThumbnailA = card.ThumbnailA,
                ThumbnailB = card.ThumbnailB,
                ContentA = card.ContentA,
                ContentB = card.ContentB,
                IsSameBothSides = card.IsSameBothSides,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"更新桌牌時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 刪除桌牌
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCard(int id)
    {
        try
        {
            Console.WriteLine($"[DeleteCard] 開始刪除桌牌，ID: {id}");
            
            var result = await _cardService.DeleteCardAsync(id);
            if (!result)
            {
                Console.WriteLine($"[DeleteCard] 找不到桌牌，ID: {id}");
                return NotFound($"找不到 ID 為 {id} 的桌牌");
            }

            Console.WriteLine($"[DeleteCard] 桌牌刪除成功，ID: {id}");
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DeleteCard] 刪除桌牌時發生錯誤，ID: {id}, 錯誤: {ex.Message}");
            return StatusCode(500, $"刪除桌牌時發生錯誤: {ex.Message}");
        }
    }
}