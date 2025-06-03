using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextTagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TextTagController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TextTag
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TextTagDto>>> GetTextTags()
        {
            var textTags = await _context.TextTags
                .Select(t => new TextTagDto
                {
                    Id = t.Id,
                    ElementId = t.ElementId,
                    CardId = t.CardId,
                    TagType = t.TagType,
                    CustomLabel = t.CustomLabel,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(textTags);
        }

        // GET: api/TextTag/card/{cardId}
        [HttpGet("card/{cardId}")]
        public async Task<ActionResult<IEnumerable<TextTagDto>>> GetTextTagsByCard(int cardId)
        {
            var textTags = await _context.TextTags
                .Where(t => t.CardId == cardId)
                .Select(t => new TextTagDto
                {
                    Id = t.Id,
                    ElementId = t.ElementId,
                    CardId = t.CardId,
                    TagType = t.TagType,
                    CustomLabel = t.CustomLabel,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(textTags);
        }

        // GET: api/TextTag/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TextTagDto>> GetTextTag(int id)
        {
            var textTag = await _context.TextTags.FindAsync(id);

            if (textTag == null)
            {
                return NotFound();
            }

            var textTagDto = new TextTagDto
            {
                Id = textTag.Id,
                ElementId = textTag.ElementId,
                CardId = textTag.CardId,
                TagType = textTag.TagType,
                CustomLabel = textTag.CustomLabel,
                Content = textTag.Content,
                CreatedAt = textTag.CreatedAt,
                UpdatedAt = textTag.UpdatedAt
            };

            return Ok(textTagDto);
        }

        // POST: api/TextTag
        [HttpPost]
        public async Task<ActionResult<TextTagDto>> CreateTextTag(CreateTextTagDto createDto)
        {
            // 檢查卡片是否存在
            var cardExists = await _context.Cards.AnyAsync(c => c.Id == createDto.CardId);
            if (!cardExists)
            {
                return BadRequest("指定的卡片不存在");
            }

            var textTag = new TextTag
            {
                ElementId = createDto.ElementId,
                CardId = createDto.CardId,
                TagType = createDto.TagType,
                CustomLabel = createDto.CustomLabel,
                Content = createDto.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TextTags.Add(textTag);
            await _context.SaveChangesAsync();

            var textTagDto = new TextTagDto
            {
                Id = textTag.Id,
                ElementId = textTag.ElementId,
                CardId = textTag.CardId,
                TagType = textTag.TagType,
                CustomLabel = textTag.CustomLabel,
                Content = textTag.Content,
                CreatedAt = textTag.CreatedAt,
                UpdatedAt = textTag.UpdatedAt
            };

            return CreatedAtAction(nameof(GetTextTag), new { id = textTag.Id }, textTagDto);
        }

        // PUT: api/TextTag/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTextTag(int id, UpdateTextTagDto updateDto)
        {
            var textTag = await _context.TextTags.FindAsync(id);
            if (textTag == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(updateDto.TagType))
                textTag.TagType = updateDto.TagType;
            
            if (updateDto.CustomLabel != null)
                textTag.CustomLabel = updateDto.CustomLabel;
            
            if (updateDto.Content != null)
                textTag.Content = updateDto.Content;

            textTag.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TextTagExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/TextTag/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTextTag(int id)
        {
            var textTag = await _context.TextTags.FindAsync(id);
            if (textTag == null)
            {
                return NotFound();
            }

            _context.TextTags.Remove(textTag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/TextTag/element/{elementId}
        [HttpDelete("element/{elementId}")]
        public async Task<IActionResult> DeleteTextTagByElement(string elementId)
        {
            var textTags = await _context.TextTags
                .Where(t => t.ElementId == elementId)
                .ToListAsync();

            if (textTags.Any())
            {
                _context.TextTags.RemoveRange(textTags);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private bool TextTagExists(int id)
        {
            return _context.TextTags.Any(e => e.Id == id);
        }
    }
} 