using Microsoft.AspNetCore.Mvc;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Services;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(ITemplateService templateService, ILogger<TemplatesController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取所有公開樣板
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateListDto>>> GetTemplates([FromQuery] string? category = null)
    {
        try
        {
            var templates = string.IsNullOrEmpty(category)
                ? await _templateService.GetPublicTemplatesAsync()
                : await _templateService.GetTemplatesByCategoryAsync(category);

            var response = templates.Select(t => new TemplateListDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ThumbnailUrl = t.ThumbnailUrl,
                ThumbnailA = t.ThumbnailA,
                ThumbnailB = t.ThumbnailB,
                Category = t.Category,
                IsPublic = t.IsPublic,
                CreatedAt = t.CreatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates");
            return StatusCode(500, $"獲取樣板時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 根據 ID 獲取樣板
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TemplateResponseDto>> GetTemplate(int id)
    {
        try
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null)
                return NotFound($"找不到 ID 為 {id} 的樣板");

            var response = new TemplateResponseDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                ThumbnailUrl = template.ThumbnailUrl,
                ThumbnailA = template.ThumbnailA,
                ThumbnailB = template.ThumbnailB,
                LayoutDataA = template.LayoutDataA,
                LayoutDataB = template.LayoutDataB,
                Dimensions = template.Dimensions,
                OrganizationId = template.OrganizationId,
                CreatedBy = template.CreatedBy,
                IsPublic = template.IsPublic,
                Category = template.Category,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                IsActive = template.IsActive
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template {TemplateId}", id);
            return StatusCode(500, $"獲取樣板時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 創建新樣板
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TemplateResponseDto>> CreateTemplate(CreateTemplateDto templateDto)
    {
        try
        {
            _logger.LogInformation("收到創建樣板請求 - 名稱: {Name}, 類別: {Category}", templateDto.Name, templateDto.Category);
            _logger.LogDebug("樣板數據: {@TemplateDto}", new { 
                templateDto.Name, 
                templateDto.Description, 
                templateDto.Category,
                templateDto.IsPublic,
                ThumbnailLength = templateDto.ThumbnailUrl?.Length ?? 0,
                LayoutDataAExists = !templateDto.LayoutDataA.Equals(default(System.Text.Json.JsonElement)),
                LayoutDataBExists = !templateDto.LayoutDataB.Equals(default(System.Text.Json.JsonElement)),
                DimensionsExists = !templateDto.Dimensions.Equals(default(System.Text.Json.JsonElement))
            });

            var template = await _templateService.CreateTemplateAsync(templateDto);
            
            _logger.LogInformation("樣板創建成功 - ID: {TemplateId}, 名稱: {Name}", template.Id, template.Name);
            
            var response = new TemplateResponseDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                ThumbnailUrl = template.ThumbnailUrl,
                ThumbnailA = template.ThumbnailA,
                ThumbnailB = template.ThumbnailB,
                LayoutDataA = template.LayoutDataA,
                LayoutDataB = template.LayoutDataB,
                Dimensions = template.Dimensions,
                OrganizationId = template.OrganizationId,
                CreatedBy = template.CreatedBy,
                IsPublic = template.IsPublic,
                Category = template.Category,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                IsActive = template.IsActive
            };

            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建樣板時發生錯誤 - 請求數據: {TemplateName}", templateDto?.Name ?? "未知");
            return StatusCode(500, $"創建樣板時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新樣板
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TemplateResponseDto>> UpdateTemplate(int id, UpdateTemplateDto templateDto)
    {
        try
        {
            var template = await _templateService.UpdateTemplateAsync(id, templateDto);
            if (template == null)
                return NotFound($"找不到 ID 為 {id} 的樣板");

            var response = new TemplateResponseDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                ThumbnailUrl = template.ThumbnailUrl,
                ThumbnailA = template.ThumbnailA,
                ThumbnailB = template.ThumbnailB,
                LayoutDataA = template.LayoutDataA,
                LayoutDataB = template.LayoutDataB,
                Dimensions = template.Dimensions,
                OrganizationId = template.OrganizationId,
                CreatedBy = template.CreatedBy,
                IsPublic = template.IsPublic,
                Category = template.Category,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                IsActive = template.IsActive
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", id);
            return StatusCode(500, $"更新樣板時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 刪除樣板
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        try
        {
            var result = await _templateService.DeleteTemplateAsync(id);
            if (!result)
                return NotFound($"找不到 ID 為 {id} 的樣板");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", id);
            return StatusCode(500, $"刪除樣板時發生錯誤: {ex.Message}");
        }
    }
}
