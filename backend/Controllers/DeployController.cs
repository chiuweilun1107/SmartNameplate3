using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeployController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DeployController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("immediate")]
    public async Task<ActionResult<DeployResultDto>> DeployImmediate(DeployRequestDto request)
    {
        var card = await _context.Cards.FindAsync(request.CardId);
        if (card == null) return BadRequest("指定的卡片不存在");

        var devices = await _context.Devices.Where(d => request.DeviceIds.Contains(d.Id)).ToListAsync();
        if (devices.Count != request.DeviceIds.Length) return BadRequest("部分設備不存在");

        var deployHistories = devices.Select(device => new DeployHistory
        {
            DeviceId = device.Id,
            CardId = request.CardId,
            Status = DeployStatus.Success,
            CreatedAt = DateTime.UtcNow,
            DeployedAt = DateTime.UtcNow,
            DeployedBy = request.DeployedBy ?? "系統"
        }).ToList();

        _context.DeployHistories.AddRange(deployHistories);
        await _context.SaveChangesAsync();

        return Ok(new DeployResultDto
        {
            Success = true,
            Message = "投送成功",
            TotalDevices = devices.Count,
            SuccessfulDeploys = devices.Count
        });
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<DeployHistoryDto>>> GetDeployHistory(
        [FromQuery] int? deviceId = null, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var query = _context.DeployHistories
            .Include(h => h.Device)
            .Include(h => h.Card)
            .AsQueryable();

        if (deviceId.HasValue)
            query = query.Where(h => h.DeviceId == deviceId.Value);

        var histories = await query
            .OrderByDescending(h => h.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(h => new DeployHistoryDto
            {
                Id = h.Id,
                DeviceId = h.DeviceId,
                CardId = h.CardId,
                Status = h.Status.ToString(),
                CreatedAt = h.CreatedAt,
                DeployedAt = h.DeployedAt,
                ErrorMessage = h.ErrorMessage,
                DeployedBy = h.DeployedBy
            })
            .ToListAsync();

        return Ok(histories);
    }
}
