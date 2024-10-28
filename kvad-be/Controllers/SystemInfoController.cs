using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SystemInfoController : Controller
{
    private SystemInfoService _systemInfoService;

    public SystemInfoController(SystemInfoService systemInfoService)
    {
        _systemInfoService = systemInfoService;
    }

    [HttpGet("cpu")]
    public async Task<IActionResult> GetCPUInfo()
    {
        return Ok(await _systemInfoService.GetCPUInfo());
    }

    [HttpGet("memory")]
    public async Task<IActionResult> GetMemoryInfo()
    {
        return Ok(await _systemInfoService.GetMemoryInfo());
    }

    [HttpGet("disk")]
    public async Task<IActionResult> GetDiskInfo()
    {
        return Ok(await _systemInfoService.GetDiskInfo());
    }

    [HttpGet("network")]
    public async Task<IActionResult> GetNetworkInfo()
    {
        return Ok(await _systemInfoService.GetNetworkInfo());
    }

    [HttpGet("uptime")]
    public async Task<IActionResult> GetUptime()
    {
        return Ok(await _systemInfoService.GetUptimeInfo());
    }

    [HttpGet("version")]
    public async Task<IActionResult> GetVersion()
    {
        return Ok(await _systemInfoService.GetSystemVersion());
    }

    [HttpGet("os")]
    public async Task<IActionResult> GetOSInfo()
    {
        return Ok(await _systemInfoService.GetOperatingSystemInfo());
    }

    [HttpGet("memoryusage")]
    public async Task<IActionResult> GetMemoryUsage()
    {
        return Ok(await _systemInfoService.GetMemoryUsage());
    }

    [HttpGet("cpuusage")]
    public async Task<IActionResult> GetCPUUsage()
    {
        return Ok(await _systemInfoService.GetCPULoad());
    }



}