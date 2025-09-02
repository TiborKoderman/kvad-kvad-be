using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MqttController : Controller
{
    private MqttServerService? _mqttServerService;

    public MqttController(MqttServerService mqttServerService)
    {
        _mqttServerService = mqttServerService;
    }

    [HttpGet("clients")]
    public async Task<IActionResult> ListMqttClients()
    {
        if (_mqttServerService == null)
        {
            return NotFound();
        }
        var clients = await _mqttServerService.GetAllActiveClients();
        if (clients == null)
        {
            return NotFound();
        }
        return Ok(clients);

    }

    [HttpGet("sessions")]
    public async Task<IActionResult> ListMqttSessions()
    {
        if (_mqttServerService == null)
        {
            return NotFound();
        }
        var sessions = await _mqttServerService.GetAllActiveSessions();
        if (sessions == null)
        {
            return NotFound();
        }
        return Ok(sessions);
    }


}