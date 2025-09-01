using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly DeviceService _deviceService;

    public DeviceController(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllDevices()
    {
        var devices = await _deviceService.GetAllDevices();
        return Ok(devices);
    }

    [HttpGet("user/all")]
    public async Task<IActionResult> GetAllDevicesOfUser()
    {
        if (HttpContext.Items["User"] is not User user)
        {
            return NotFound("User not found.");
        }

        var devices = await _deviceService.GetAllDevicesOfUser(user);
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeviceById(Guid id)
    {
        var device = await _deviceService.GetDeviceById(id);
        if (device == null)
        {
            return NotFound("Device not found.");
        }

        return Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> AddDevice([FromBody] Device device)
    {
        await _deviceService.AddDevice(device);
        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
    }

    [HttpPut("virtual")]
    public async Task<IActionResult> EditVirtualDevice(VirtualDeviceDTO deviceDTO)
    {
        if (HttpContext.Items["User"] is not User user)
        {
            return NotFound("User not found.");
        }

        var device = new Device
        {
            Id = deviceDTO.Id ?? Guid.NewGuid(),
            Name = deviceDTO.Name,
            Description = deviceDTO.Description ?? string.Empty,
            Virtual = true,
            Owner = user,
            OwnerId = user.Id,
            Groups = user.PrivateGroup != null ? [user.PrivateGroup] : [], // Use List<Group>
            State = new DeviceState(),
        };

        await _deviceService.AddDevice(device);
        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDevice(Guid id, [FromBody] Device device)
    {
        if (id != device.Id)
        {
            return BadRequest("Device ID mismatch.");
        }

        await _deviceService.UpdateDevice(device);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        var device = await _deviceService.GetDeviceById(id);
        if (device == null)
        {
            return NotFound("Device not found.");
        }

        await _deviceService.DeleteDevice(id);
        return NoContent();
    }

    [HttpGet("tagSources")]
    public async Task<IActionResult> GetTagSources()
    {
        var tagSources = await _deviceService.GetAllTagSources();
        return Ok(tagSources);
    }

    [HttpGet("historizationIntervals")]
    public async Task<IActionResult> GetHistoricizationIntervals()
    {
        var intervals = await _deviceService.GetHistorizationIntervals();
        return Ok(intervals);
    }

}
