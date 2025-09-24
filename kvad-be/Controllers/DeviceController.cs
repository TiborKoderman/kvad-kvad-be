using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DeviceController(DeviceService deviceService, AuthService authService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAllDevices()
    {
        var devices = await deviceService.GetAllDevices();
        return Ok(devices);
    }

    [HttpGet("user/all")]
    public async Task<IActionResult> GetAllDevicesOfUser()
    {
        if (await authService.GetUser(User) is not User user)
        {
            return NotFound("User not found.");
        }

        var devices = await deviceService.GetAllDevicesOfUser(user);
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeviceById(Guid id)
    {
        var device = await deviceService.GetDeviceById(id);
        if (device == null)
        {
            return NotFound("Device not found.");
        }

        return Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> AddDevice([FromBody] Device device)
    {
        await deviceService.AddDevice(device);
        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
    }

    [HttpPut("virtual")]
    public async Task<IActionResult> EditVirtualDevice(VirtualDeviceDTO deviceDTO)
    {
        var user = await authService.GetUser(User);
        if (user == null)
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
            Info = new DeviceInfo() // Set the required Info property
        };

        await deviceService.AddDevice(device);
        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDevice(Guid id, [FromBody] Device device)
    {
        if (id != device.Id)
        {
            return BadRequest("Device ID mismatch.");
        }

        await deviceService.UpdateDevice(device);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        var device = await deviceService.GetDeviceById(id);
        if (device == null)
        {
            return NotFound("Device not found.");
        }

        await deviceService.DeleteDevice(id);
        return NoContent();
    }

    [HttpGet("tagSources")]
    public async Task<IActionResult> GetTagSources()
    {
        var tagSources = await deviceService.GetAllTagSources();
        return Ok(tagSources);
    }

    [HttpPost("{id}/heartbeat")]
    public async Task<IActionResult> ProcessHeartbeat(Guid id, [FromBody] HeartbeatDTO heartbeat)
    {
        try
        {
            await deviceService.ProcessHeartbeatAsync(id, heartbeat);
            return Ok(new { message = "Heartbeat processed successfully", deviceId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error processing heartbeat", error = ex.Message });
        }
    }

    [HttpGet("stale")]
    public async Task<IActionResult> GetStaleDevices([FromQuery] int minutesThreshold = 5)
    {
        var staleThreshold = TimeSpan.FromMinutes(minutesThreshold);
        var staleDevices = await deviceService.GetDevicesWithStaleHeartbeats(staleThreshold);

        return Ok(staleDevices.Select(d => new
        {
            Id = d.Id,
            Name = d.Name,
            LastHeartbeat = d.State?.LastHeartbeat,
            Connectivity = d.State?.Connectivity,
            Health = d.State?.Health
        }));
    }


}
