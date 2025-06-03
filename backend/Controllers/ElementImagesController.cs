using Microsoft.AspNetCore.Mvc;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Services;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElementImagesController : ControllerBase
{
    private readonly IElementImageService _elementImageService;
    private readonly ILogger<ElementImagesController> _logger;

    public ElementImagesController(
        IElementImageService elementImageService,
        ILogger<ElementImagesController> logger)
    {
        _elementImageService = elementImageService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取所有公開圖片元素
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ElementImageResponseDto>>> GetElementImages([FromQuery] string? category = null)
    {
        try
        {
            var elementImages = string.IsNullOrEmpty(category)
                ? await _elementImageService.GetPublicElementImagesAsync()
                : await _elementImageService.GetElementImagesByCategoryAsync(category);

            var response = elementImages.Select(ei => new ElementImageResponseDto
            {
                Id = ei.Id,
                Name = ei.Name,
                Description = ei.Description,
                ImageUrl = ei.ImageUrl,
                ThumbnailUrl = ei.ThumbnailUrl,
                Category = ei.Category,
                IsPublic = ei.IsPublic,
                CreatedBy = ei.CreatedBy,
                CreatedAt = ei.CreatedAt,
                UpdatedAt = ei.UpdatedAt,
                IsActive = ei.IsActive
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取圖片元素時發生錯誤");
            return StatusCode(500, "獲取圖片元素時發生錯誤");
        }
    }

    /// <summary>
    /// 根據ID獲取圖片元素
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ElementImageResponseDto>> GetElementImage(int id)
    {
        try
        {
            var elementImage = await _elementImageService.GetElementImageByIdAsync(id);
            if (elementImage == null)
                return NotFound($"找不到 ID 為 {id} 的圖片元素");

            var response = new ElementImageResponseDto
            {
                Id = elementImage.Id,
                Name = elementImage.Name,
                Description = elementImage.Description,
                ImageUrl = elementImage.ImageUrl,
                ThumbnailUrl = elementImage.ThumbnailUrl,
                Category = elementImage.Category,
                IsPublic = elementImage.IsPublic,
                CreatedBy = elementImage.CreatedBy,
                CreatedAt = elementImage.CreatedAt,
                UpdatedAt = elementImage.UpdatedAt,
                IsActive = elementImage.IsActive
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取圖片元素 {Id} 時發生錯誤", id);
            return StatusCode(500, "獲取圖片元素時發生錯誤");
        }
    }

    /// <summary>
    /// 創建新圖片元素
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ElementImageResponseDto>> CreateElementImage(CreateElementImageDto elementImageDto)
    {
        try
        {
            var elementImage = await _elementImageService.CreateElementImageAsync(elementImageDto);

            var response = new ElementImageResponseDto
            {
                Id = elementImage.Id,
                Name = elementImage.Name,
                Description = elementImage.Description,
                ImageUrl = elementImage.ImageUrl,
                ThumbnailUrl = elementImage.ThumbnailUrl,
                Category = elementImage.Category,
                IsPublic = elementImage.IsPublic,
                CreatedBy = elementImage.CreatedBy,
                CreatedAt = elementImage.CreatedAt,
                UpdatedAt = elementImage.UpdatedAt,
                IsActive = elementImage.IsActive
            };

            return CreatedAtAction(nameof(GetElementImage), new { id = elementImage.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建圖片元素時發生錯誤");
            return StatusCode(500, "創建圖片元素時發生錯誤");
        }
    }

    /// <summary>
    /// 更新圖片元素
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ElementImageResponseDto>> UpdateElementImage(int id, UpdateElementImageDto elementImageDto)
    {
        try
        {
            var elementImage = await _elementImageService.UpdateElementImageAsync(id, elementImageDto);
            if (elementImage == null)
                return NotFound($"找不到 ID 為 {id} 的圖片元素");

            var response = new ElementImageResponseDto
            {
                Id = elementImage.Id,
                Name = elementImage.Name,
                Description = elementImage.Description,
                ImageUrl = elementImage.ImageUrl,
                ThumbnailUrl = elementImage.ThumbnailUrl,
                Category = elementImage.Category,
                IsPublic = elementImage.IsPublic,
                CreatedBy = elementImage.CreatedBy,
                CreatedAt = elementImage.CreatedAt,
                UpdatedAt = elementImage.UpdatedAt,
                IsActive = elementImage.IsActive
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新圖片元素 {Id} 時發生錯誤", id);
            return StatusCode(500, "更新圖片元素時發生錯誤");
        }
    }

    /// <summary>
    /// 刪除圖片元素
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteElementImage(int id)
    {
        try
        {
            var result = await _elementImageService.DeleteElementImageAsync(id);
            if (!result)
                return NotFound($"找不到 ID 為 {id} 的圖片元素");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除圖片元素 {Id} 時發生錯誤", id);
            return StatusCode(500, "刪除圖片元素時發生錯誤");
        }
    }
} 