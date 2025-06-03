using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Services;

public interface IBackgroundImageService
{
    Task<IEnumerable<BackgroundImage>> GetAllBackgroundImagesAsync();
    Task<IEnumerable<BackgroundImage>> GetPublicBackgroundImagesAsync();
    Task<IEnumerable<BackgroundImage>> GetBackgroundImagesByCategoryAsync(string category);
    Task<BackgroundImage?> GetBackgroundImageByIdAsync(int id);
    Task<BackgroundImage> CreateBackgroundImageAsync(CreateBackgroundImageDto backgroundImageDto);
    Task<BackgroundImage?> UpdateBackgroundImageAsync(int id, UpdateBackgroundImageDto backgroundImageDto);
    Task<bool> DeleteBackgroundImageAsync(int id);
}

public class BackgroundImageService : IBackgroundImageService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BackgroundImageService> _logger;

    public BackgroundImageService(ApplicationDbContext context, ILogger<BackgroundImageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<BackgroundImage>> GetAllBackgroundImagesAsync()
    {
        return await _context.BackgroundImages
            .Where(bg => bg.IsActive)
            .Include(bg => bg.Creator)
            .OrderByDescending(bg => bg.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BackgroundImage>> GetPublicBackgroundImagesAsync()
    {
        return await _context.BackgroundImages
            .Where(bg => bg.IsActive && bg.IsPublic)
            .Include(bg => bg.Creator)
            .OrderByDescending(bg => bg.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BackgroundImage>> GetBackgroundImagesByCategoryAsync(string category)
    {
        return await _context.BackgroundImages
            .Where(bg => bg.IsActive && bg.IsPublic && bg.Category == category)
            .Include(bg => bg.Creator)
            .OrderByDescending(bg => bg.CreatedAt)
            .ToListAsync();
    }

    public async Task<BackgroundImage?> GetBackgroundImageByIdAsync(int id)
    {
        return await _context.BackgroundImages
            .Include(bg => bg.Creator)
            .FirstOrDefaultAsync(bg => bg.Id == id && bg.IsActive);
    }

    public async Task<BackgroundImage> CreateBackgroundImageAsync(CreateBackgroundImageDto backgroundImageDto)
    {
        var backgroundImage = new BackgroundImage
        {
            Name = backgroundImageDto.Name,
            Description = backgroundImageDto.Description,
            ImageUrl = backgroundImageDto.ImageUrl,
            ThumbnailUrl = backgroundImageDto.ThumbnailUrl,
            Category = backgroundImageDto.Category ?? "general",
            IsPublic = backgroundImageDto.IsPublic,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.BackgroundImages.Add(backgroundImage);
        await _context.SaveChangesAsync();

        return backgroundImage;
    }

    public async Task<BackgroundImage?> UpdateBackgroundImageAsync(int id, UpdateBackgroundImageDto backgroundImageDto)
    {
        var backgroundImage = await _context.BackgroundImages.FindAsync(id);
        if (backgroundImage == null || !backgroundImage.IsActive)
            return null;

        if (!string.IsNullOrEmpty(backgroundImageDto.Name))
            backgroundImage.Name = backgroundImageDto.Name;

        if (backgroundImageDto.Description != null)
            backgroundImage.Description = backgroundImageDto.Description;

        if (backgroundImageDto.ImageUrl != null)
            backgroundImage.ImageUrl = backgroundImageDto.ImageUrl;

        if (backgroundImageDto.ThumbnailUrl != null)
            backgroundImage.ThumbnailUrl = backgroundImageDto.ThumbnailUrl;

        if (backgroundImageDto.Category != null)
            backgroundImage.Category = backgroundImageDto.Category;

        if (backgroundImageDto.IsPublic.HasValue)
            backgroundImage.IsPublic = backgroundImageDto.IsPublic.Value;

        if (backgroundImageDto.IsActive.HasValue)
            backgroundImage.IsActive = backgroundImageDto.IsActive.Value;

        backgroundImage.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return backgroundImage;
    }

    public async Task<bool> DeleteBackgroundImageAsync(int id)
    {
        var backgroundImage = await _context.BackgroundImages.FindAsync(id);
        if (backgroundImage == null)
            return false;

        // 硬刪除 - 從資料庫中完全移除
        _context.BackgroundImages.Remove(backgroundImage);
        await _context.SaveChangesAsync();
        return true;
    }
}
