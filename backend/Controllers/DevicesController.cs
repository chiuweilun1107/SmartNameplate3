using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Data;
using SmartNameplate.Api.DTOs;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(ApplicationDbContext context, ILogger<DevicesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevices()
    {
        try
        {
            var devices = await _context.Devices
                .Include(d => d.CurrentCard)
                .Include(d => d.Group)
                .OrderBy(d => d.Name)
                .Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    BluetoothAddress = d.BluetoothAddress,
                    Status = d.Status.ToString(),
                    CurrentCardId = d.CurrentCardId,
                    CurrentCardName = d.CurrentCard != null ? d.CurrentCard.Name : null,
                    GroupId = d.GroupId,
                    GroupName = d.Group != null ? d.Group.Name : null,
                    LastConnected = d.LastConnected,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    CustomIndex = d.CustomIndex
                })
                .ToListAsync();

            return Ok(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving devices");
            return StatusCode(500, "An error occurred while retrieving devices");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDto>> GetDevice(int id)
    {
        try
        {
            var device = await _context.Devices
                .Include(d => d.CurrentCard)
                .Include(d => d.Group)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound();
            }

            var deviceDto = new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                BluetoothAddress = device.BluetoothAddress,
                Status = device.Status.ToString(),
                CurrentCardId = device.CurrentCardId,
                CurrentCardName = device.CurrentCard?.Name,
                GroupId = device.GroupId,
                GroupName = device.Group?.Name,
                LastConnected = device.LastConnected,
                CreatedAt = device.CreatedAt,
                UpdatedAt = device.UpdatedAt,
                CustomIndex = device.CustomIndex
            };

            return Ok(deviceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving device {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the device");
        }
    }

    [HttpPost]
    public async Task<ActionResult<DeviceDto>> CreateDevice(CreateDeviceDto createDto)
    {
        try
        {
            var device = new Device
            {
                Name = createDto.Name,
                BluetoothAddress = createDto.BluetoothAddress,
                GroupId = createDto.GroupId,
                Status = DeviceStatus.Disconnected,
                LastConnected = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            var deviceDto = new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                BluetoothAddress = device.BluetoothAddress,
                Status = device.Status.ToString(),
                CurrentCardId = device.CurrentCardId,
                GroupId = device.GroupId,
                LastConnected = device.LastConnected,
                CreatedAt = device.CreatedAt,
                UpdatedAt = device.UpdatedAt
            };

            return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, deviceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            return StatusCode(500, "An error occurred while creating the device");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDevice(int id, UpdateDeviceDto updateDto)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            device.Name = updateDto.Name;
            device.GroupId = updateDto.GroupId;
            device.CustomIndex = updateDto.CustomIndex;
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device {Id}", id);
            return StatusCode(500, "An error occurred while updating the device");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        try
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device {Id}", id);
            return StatusCode(500, "An error occurred while deleting the device");
        }
    }
}
