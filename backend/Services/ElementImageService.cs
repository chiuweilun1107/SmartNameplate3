using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Services;

public interface IElementImageService
{
    Task<IEnumerable<ElementImage>> GetAllElementImagesAsync();
    Task<IEnumerable<ElementImage>> GetPublicElementImagesAsync();
    Task<IEnumerable<ElementImage>> GetElementImagesByCategoryAsync(string category);
    Task<ElementImage?> GetElementImageByIdAsync(int id);
    Task<ElementImage> CreateElementImageAsync(CreateElementImageDto elementImageDto);
    Task<ElementImage?> UpdateElementImageAsync(int id, UpdateElementImageDto elementImageDto);
    Task<bool> DeleteElementImageAsync(int id);
}

public class ElementImageService : IElementImageService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ElementImageService> _logger;

    public ElementImageService(ApplicationDbContext context, ILogger<ElementImageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ElementImage>> GetAllElementImagesAsync()
    {
        return await _context.ElementImages
            .Where(ei => ei.IsActive)
            .Include(ei => ei.Creator)
            .OrderByDescending(ei => ei.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ElementImage>> GetPublicElementImagesAsync()
    {
        return await _context.ElementImages
            .Where(ei => ei.IsActive && ei.IsPublic)
            .Include(ei => ei.Creator)
            .OrderByDescending(ei => ei.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ElementImage>> GetElementImagesByCategoryAsync(string category)
    {
        return await _context.ElementImages
            .Where(ei => ei.IsActive && ei.IsPublic && ei.Category == category)
            .Include(ei => ei.Creator)
            .OrderByDescending(ei => ei.CreatedAt)
            .ToListAsync();
    }

    public async Task<ElementImage?> GetElementImageByIdAsync(int id)
    {
        return await _context.ElementImages
            .Include(ei => ei.Creator)
            .FirstOrDefaultAsync(ei => ei.Id == id);
    }

    public async Task<ElementImage> CreateElementImageAsync(CreateElementImageDto elementImageDto)
    {
        var elementImage = new ElementImage
        {
            Name = elementImageDto.Name,
            Description = elementImageDto.Description,
            ImageUrl = elementImageDto.ImageUrl,
            ThumbnailUrl = elementImageDto.ThumbnailUrl,
            Category = elementImageDto.Category ?? "general",
            IsPublic = elementImageDto.IsPublic,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ElementImages.Add(elementImage);
        await _context.SaveChangesAsync();

        return elementImage;
    }

    public async Task<ElementImage?> UpdateElementImageAsync(int id, UpdateElementImageDto elementImageDto)
    {
        var elementImage = await _context.ElementImages.FindAsync(id);
        if (elementImage == null || !elementImage.IsActive)
            return null;

        if (!string.IsNullOrEmpty(elementImageDto.Name))
            elementImage.Name = elementImageDto.Name;

        if (elementImageDto.Description != null)
            elementImage.Description = elementImageDto.Description;

        if (elementImageDto.ImageUrl != null)
            elementImage.ImageUrl = elementImageDto.ImageUrl;

        if (elementImageDto.ThumbnailUrl != null)
            elementImage.ThumbnailUrl = elementImageDto.ThumbnailUrl;

        if (elementImageDto.Category != null)
            elementImage.Category = elementImageDto.Category;

        if (elementImageDto.IsPublic.HasValue)
            elementImage.IsPublic = elementImageDto.IsPublic.Value;

        if (elementImageDto.IsActive.HasValue)
            elementImage.IsActive = elementImageDto.IsActive.Value;

        elementImage.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return elementImage;
    }

    public async Task<bool> DeleteElementImageAsync(int id)
    {
        var elementImage = await _context.ElementImages.FindAsync(id);
        if (elementImage == null)
            return false;

        _context.ElementImages.Remove(elementImage);
        await _context.SaveChangesAsync();

        return true;
    }
} 