using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Services;

public interface ITemplateService
{
    Task<IEnumerable<Template>> GetAllTemplatesAsync();
    Task<IEnumerable<Template>> GetPublicTemplatesAsync();
    Task<IEnumerable<Template>> GetTemplatesByCategoryAsync(string category);
    Task<Template?> GetTemplateByIdAsync(int id);
    Task<Template> CreateTemplateAsync(CreateTemplateDto templateDto);
    Task<Template?> UpdateTemplateAsync(int id, UpdateTemplateDto templateDto);
    Task<bool> DeleteTemplateAsync(int id);
}

public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(ApplicationDbContext context, ILogger<TemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Template>> GetAllTemplatesAsync()
    {
        return await _context.Templates
            .Where(t => t.IsActive)
            .Include(t => t.Creator)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetPublicTemplatesAsync()
    {
        return await _context.Templates
            .Where(t => t.IsActive && t.IsPublic)
            .Include(t => t.Creator)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetTemplatesByCategoryAsync(string category)
    {
        var query = _context.Templates
            .Where(t => t.IsActive && t.IsPublic);

        if (!string.IsNullOrEmpty(category) && category != "全部")
        {
            query = query.Where(t => t.Category == category);
        }

        return await query
            .Include(t => t.Creator)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Template?> GetTemplateByIdAsync(int id)
    {
        return await _context.Templates
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
    }

    public async Task<Template> CreateTemplateAsync(CreateTemplateDto templateDto)
    {
        var template = new Template
        {
            Name = templateDto.Name,
            Description = templateDto.Description,
            ThumbnailUrl = templateDto.ThumbnailUrl,
            ThumbnailA = templateDto.ThumbnailA,
            ThumbnailB = templateDto.ThumbnailB,
            LayoutDataA = templateDto.LayoutDataA,
            LayoutDataB = templateDto.LayoutDataB,
            Dimensions = templateDto.Dimensions,
            IsPublic = templateDto.IsPublic,
            Category = templateDto.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Templates.Add(template);
        await _context.SaveChangesAsync();

        return template;
    }

    public async Task<Template?> UpdateTemplateAsync(int id, UpdateTemplateDto templateDto)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null || !template.IsActive)
            return null;

        if (!string.IsNullOrEmpty(templateDto.Name))
            template.Name = templateDto.Name;

        if (templateDto.Description != null)
            template.Description = templateDto.Description;

        if (templateDto.ThumbnailUrl != null)
            template.ThumbnailUrl = templateDto.ThumbnailUrl;

        if (templateDto.ThumbnailA != null)
            template.ThumbnailA = templateDto.ThumbnailA;

        if (templateDto.ThumbnailB != null)
            template.ThumbnailB = templateDto.ThumbnailB;

        if (templateDto.LayoutDataA != null)
            template.LayoutDataA = templateDto.LayoutDataA;

        if (templateDto.LayoutDataB != null)
            template.LayoutDataB = templateDto.LayoutDataB;

        if (templateDto.Dimensions != null)
            template.Dimensions = templateDto.Dimensions;

        if (templateDto.IsPublic.HasValue)
            template.IsPublic = templateDto.IsPublic.Value;

        if (!string.IsNullOrEmpty(templateDto.Category))
            template.Category = templateDto.Category;

        if (templateDto.IsActive.HasValue)
            template.IsActive = templateDto.IsActive.Value;

        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return template;
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null)
            return false;

        // 硬刪除 - 從資料庫中完全移除
        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();
        return true;
    }
}
