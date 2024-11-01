using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SystemServiceManagmentController : Controller
{
    private SystemServiceManagmentService _systemServiceManagmentService;

    public SystemServiceManagmentController(SystemServiceManagmentService systemServiceManagmentService)
    {
        _systemServiceManagmentService = systemServiceManagmentService;
    }

    [HttpGet("processes")]
    public async Task<IActionResult> GetProcesses()
    {
        return Ok(await _systemServiceManagmentService.GetProcessList());
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetServices(String? type, String? status)
    {
        return Ok(await _systemServiceManagmentService.GetServiceList(type, status));
    }

    [HttpGet("service")]
    public async Task<IActionResult> GetService(String name)
    {
        return Ok(await _systemServiceManagmentService.GetService(name));
    }

    [HttpGet("service/is_active")]
    public async Task<IActionResult> GetServiceStatus(String name)
    {
        var JsonObject = await _systemServiceManagmentService.GetService(name);
        return Ok(JsonObject["ActiveState"]);
    }

    [HttpGet("service/start")]
    public async Task<IActionResult> StartService(String name)
    {
        return Ok(await _systemServiceManagmentService.StartService(name));
    }

    [HttpGet("service/stop")]
    public async Task<IActionResult> StopService(String name)
    {
        return Ok(await _systemServiceManagmentService.StopService(name));
    }

    [HttpGet("service/restart")]
    public async Task<IActionResult> RestartService(String name)
    {
        return Ok(await _systemServiceManagmentService.RestartService(name));
    }

    [HttpGet("service/enable")]
    public async Task<IActionResult> EnableService(String name)
    {
        return Ok(await _systemServiceManagmentService.EnableService(name));
    }

    [HttpGet("service/disable")]
    public async Task<IActionResult> DisableService(String name)
    {
        return Ok(await _systemServiceManagmentService.DisableService(name));
    }
}