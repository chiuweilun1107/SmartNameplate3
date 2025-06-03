using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.Entities;
using SmartNameplate.Api.DTOs;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(ApplicationDbContext context, ILogger<GroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/groups
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
    {
        try
        {
            var groups = await _context.Groups
                .Include(g => g.GroupCards)
                .ThenInclude(gc => gc.Card)
                .Include(g => g.Devices)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Color = g.Color,
                    CardCount = g.GroupCards.Count,
                    DeviceCount = g.Devices.Count,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt
                })
                .ToListAsync();

            return Ok(groups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving groups");
            return StatusCode(500, "An error occurred while retrieving groups");
        }
    }

    // GET: api/groups/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDetailDto>> GetGroup(int id)
    {
        try
        {
            var group = await _context.Groups
                .Include(g => g.GroupCards)
                .ThenInclude(gc => gc.Card)
                .Include(g => g.Devices)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            var groupDetail = new GroupDetailDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Color = group.Color,
                Cards = group.GroupCards.Select(gc => new CardResponseDto
                {
                    Id = gc.Card.Id,
                    Name = gc.Card.Name,
                    Description = gc.Card.Description,
                    Status = gc.Card.Status,
                    ThumbnailA = gc.Card.ThumbnailA,
                    ThumbnailB = gc.Card.ThumbnailB,
                    ContentA = gc.Card.ContentA,
                    ContentB = gc.Card.ContentB,
                    CreatedAt = gc.Card.CreatedAt,
                    UpdatedAt = gc.Card.UpdatedAt
                }).ToList(),
                Devices = group.Devices.Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    BluetoothAddress = d.BluetoothAddress,
                    Status = d.Status.ToString(),
                    CurrentCardId = d.CurrentCardId,
                    LastConnected = d.LastConnected
                }).ToList(),
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };

            return Ok(groupDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving group {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the group");
        }
    }

    // POST: api/groups
    [HttpPost]
    public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto createGroupDto)
    {
        try
        {
            var group = new Group
            {
                Name = createGroupDto.Name,
                Description = createGroupDto.Description,
                Color = createGroupDto.Color ?? "#007ACC",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Color = group.Color,
                CardCount = 0,
                DeviceCount = 0,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, groupDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group");
            return StatusCode(500, "An error occurred while creating the group");
        }
    }

    // PUT: api/groups/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(int id, UpdateGroupDto updateGroupDto)
    {
        try
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            group.Name = updateGroupDto.Name;
            group.Description = updateGroupDto.Description;
            group.Color = updateGroupDto.Color ?? group.Color;
            group.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {Id}", id);
            return StatusCode(500, "An error occurred while updating the group");
        }
    }

    // DELETE: api/groups/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        try
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting group {Id}", id);
            return StatusCode(500, "An error occurred while deleting the group");
        }
    }

    // POST: api/groups/{id}/cards
    [HttpPost("{id}/cards")]
    public async Task<IActionResult> AddCardToGroup(int id, AddCardToGroupDto addCardDto)
    {
        try
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound("Group not found");
            }

            var card = await _context.Cards.FindAsync(addCardDto.CardId);
            if (card == null)
            {
                return NotFound("Card not found");
            }

            var existingGroupCard = await _context.GroupCards
                .FirstOrDefaultAsync(gc => gc.GroupId == id && gc.CardId == addCardDto.CardId);

            if (existingGroupCard != null)
            {
                return BadRequest("Card is already in this group");
            }

            var groupCard = new GroupCard
            {
                GroupId = id,
                CardId = addCardDto.CardId,
                CreatedAt = DateTime.UtcNow
            };

            _context.GroupCards.Add(groupCard);
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card to group");
            return StatusCode(500, "An error occurred while adding card to group");
        }
    }

    // DELETE: api/groups/{id}/cards/{cardId}
    [HttpDelete("{id}/cards/{cardId}")]
    public async Task<IActionResult> RemoveCardFromGroup(int id, int cardId)
    {
        try
        {
            var groupCard = await _context.GroupCards
                .FirstOrDefaultAsync(gc => gc.GroupId == id && gc.CardId == cardId);

            if (groupCard == null)
            {
                return NotFound();
            }

            _context.GroupCards.Remove(groupCard);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing card from group");
            return StatusCode(500, "An error occurred while removing card from group");
        }
    }
} 