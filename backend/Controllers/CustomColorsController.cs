using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomColorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomColorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomColor>>> GetCustomColors()
        {
            var colors = await _context.CustomColors
                .Where(c => c.IsPublic)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(colors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomColor>> GetCustomColor(int id)
        {
            var color = await _context.CustomColors.FindAsync(id);

            if (color == null)
            {
                return NotFound();
            }

            return Ok(color);
        }

        [HttpPost]
        public async Task<ActionResult<CustomColor>> CreateCustomColor(CreateCustomColorDto dto)
        {
            // 檢查顏色是否已存在
            var existingColor = await _context.CustomColors
                .FirstOrDefaultAsync(c => c.ColorValue == dto.ColorValue);

            if (existingColor != null)
            {
                return BadRequest(new { message = "此顏色已存在" });
            }

            var customColor = new CustomColor
            {
                Name = dto.Name,
                ColorValue = dto.ColorValue,
                CreatedBy = dto.CreatedBy ?? "system",
                IsPublic = dto.IsPublic
            };

            _context.CustomColors.Add(customColor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomColor), new { id = customColor.Id }, customColor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomColor(int id, UpdateCustomColorDto dto)
        {
            var color = await _context.CustomColors.FindAsync(id);

            if (color == null)
            {
                return NotFound();
            }

            color.Name = dto.Name ?? color.Name;
            color.ColorValue = dto.ColorValue ?? color.ColorValue;
            color.IsPublic = dto.IsPublic ?? color.IsPublic;
            color.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomColor(int id)
        {
            var color = await _context.CustomColors.FindAsync(id);

            if (color == null)
            {
                return NotFound();
            }

            _context.CustomColors.Remove(color);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateCustomColorDto
    {
        public string Name { get; set; } = string.Empty;
        public string ColorValue { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public bool IsPublic { get; set; } = true;
    }

    public class UpdateCustomColorDto
    {
        public string? Name { get; set; }
        public string? ColorValue { get; set; }
        public bool? IsPublic { get; set; }
    }
} 