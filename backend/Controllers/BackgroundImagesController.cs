using Microsoft.AspNetCore.Mvc;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Services;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackgroundImagesController : ControllerBase
{
    private readonly IBackgroundImageService _backgroundImageService;
    private readonly ILogger<BackgroundImagesController> _logger;

    public BackgroundImagesController(
        IBackgroundImageService backgroundImageService,
        ILogger<BackgroundImagesController> logger)
    {
        _backgroundImageService = backgroundImageService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取所有公開背景圖片
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BackgroundImageResponseDto>>> GetBackgroundImages([FromQuery] string? category = null)
    {
        try
        {
            var backgroundImages = string.IsNullOrEmpty(category)
                ? await _backgroundImageService.GetPublicBackgroundImagesAsync()
                : await _backgroundImageService.GetBackgroundImagesByCategoryAsync(category);

            var response = backgroundImages.Select(bg => new BackgroundImageResponseDto
            {
                Id = bg.Id,
                Name = bg.Name,
                Description = bg.Description,
                ImageUrl = bg.ImageUrl,
                ThumbnailUrl = bg.ThumbnailUrl,
                Category = bg.Category,
                IsPublic = bg.IsPublic,
                CreatedBy = bg.CreatedBy,
                CreatedAt = bg.CreatedAt,
                UpdatedAt = bg.UpdatedAt,
                IsActive = bg.IsActive
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取背景圖片時發生錯誤");
            return StatusCode(500, "獲取背景圖片時發生錯誤");
        }
    }

    /// <summary>
    /// 根據ID獲取背景圖片
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BackgroundImageResponseDto>> GetBackgroundImage(int id)
    {
        try
        {
            var backgroundImage = await _backgroundImageService.GetBackgroundImageByIdAsync(id);
            if (backgroundImage == null)
                return NotFound($"找不到 ID 為 {id} 的背景圖片");

            var response = new BackgroundImageResponseDto
            {
                Id = backgroundImage.Id,
                Name = backgroundImage.Name,
                Description = backgroundImage.Description,
                ImageUrl = backgroundImage.ImageUrl,
                ThumbnailUrl = backgroundImage.ThumbnailUrl,
                Category = backgroundImage.Category,
                IsPublic = backgroundImage.IsPublic,
                CreatedBy = backgroundImage.CreatedBy,
                CreatedAt = backgroundImage.CreatedAt,
                UpdatedAt = backgroundImage.UpdatedAt,
                IsActive = backgroundImage.IsActive
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取背景圖片 {Id} 時發生錯誤", id);
            return StatusCode(500, "獲取背景圖片時發生錯誤");
        }
    }

    /// <summary>
    /// 創建新背景圖片
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BackgroundImageResponseDto>> CreateBackgroundImage(CreateBackgroundImageDto backgroundImageDto)
    {
        try
        {
            var backgroundImage = await _backgroundImageService.CreateBackgroundImageAsync(backgroundImageDto);
            var response = new BackgroundImageResponseDto
            {
                Id = backgroundImage.Id,
                Name = backgroundImage.Name,
                Description = backgroundImage.Description,
                ImageUrl = backgroundImage.ImageUrl,
                ThumbnailUrl = backgroundImage.ThumbnailUrl,
                Category = backgroundImage.Category,
                IsPublic = backgroundImage.IsPublic,
                CreatedBy = backgroundImage.CreatedBy,
                CreatedAt = backgroundImage.CreatedAt,
                UpdatedAt = backgroundImage.UpdatedAt,
                IsActive = backgroundImage.IsActive
            };

            return CreatedAtAction(nameof(GetBackgroundImage), new { id = backgroundImage.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建背景圖片時發生錯誤");
            return StatusCode(500, "創建背景圖片時發生錯誤");
        }
    }

    /// <summary>
    /// 更新背景圖片
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BackgroundImageResponseDto>> UpdateBackgroundImage(int id, UpdateBackgroundImageDto backgroundImageDto)
    {
        try
        {
            var backgroundImage = await _backgroundImageService.UpdateBackgroundImageAsync(id, backgroundImageDto);
            if (backgroundImage == null)
                return NotFound($"找不到 ID 為 {id} 的背景圖片");

            var response = new BackgroundImageResponseDto
            {
                Id = backgroundImage.Id,
                Name = backgroundImage.Name,
                Description = backgroundImage.Description,
                ImageUrl = backgroundImage.ImageUrl,
                ThumbnailUrl = backgroundImage.ThumbnailUrl,
                Category = backgroundImage.Category,
                IsPublic = backgroundImage.IsPublic,
                CreatedBy = backgroundImage.CreatedBy,
                CreatedAt = backgroundImage.CreatedAt,
                UpdatedAt = backgroundImage.UpdatedAt,
                IsActive = backgroundImage.IsActive
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新背景圖片 {Id} 時發生錯誤", id);
            return StatusCode(500, "更新背景圖片時發生錯誤");
        }
    }

    /// <summary>
    /// 刪除背景圖片
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBackgroundImage(int id)
    {
        try
        {
            var result = await _backgroundImageService.DeleteBackgroundImageAsync(id);
            if (!result)
                return NotFound($"找不到 ID 為 {id} 的背景圖片");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除背景圖片 {Id} 時發生錯誤", id);
            return StatusCode(500, "刪除背景圖片時發生錯誤");
        }
    }
}
