using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SystemInfoController(SystemInfoService systemInfoService) : Controller
{
    [HttpGet("cpu")]
    public async Task<IActionResult> GetCPUInfo()
    {
        return Ok(await systemInfoService.GetCPUInfo());
    }

    [HttpGet("memory")]
    public async Task<IActionResult> GetMemoryInfo()
    {
        return Ok(await systemInfoService.GetMemoryInfo());
    }

    [HttpGet("disk")]
    public async Task<IActionResult> GetDiskInfo()
    {
        return Ok(await systemInfoService.GetDiskInfo());
    }

    [HttpGet("network")]
    public async Task<IActionResult> GetNetworkInfo()
    {
        return Ok(await systemInfoService.GetNetworkInfo());
    }

    [HttpGet("uptime")]
    public async Task<IActionResult> GetUptime()
    {
        return Ok(await systemInfoService.GetUptimeInfo());
    }

    [HttpGet("version")]
    public async Task<IActionResult> GetVersion()
    {
        return Ok(await systemInfoService.GetSystemVersion());
    }

    [HttpGet("os")]
    public async Task<IActionResult> GetOSInfo()
    {
        return Ok(await systemInfoService.GetOperatingSystemInfo());
    }

    [HttpGet("memoryusage")]
    public async Task<IActionResult> GetMemoryUsage()
    {
        return Ok(await systemInfoService.GetMemoryUsage());
    }

    [HttpGet("cpuusage")]
    public async Task<IActionResult> GetCPUUsage()
    {
        return Ok(await systemInfoService.GetCPULoad());
    }



}