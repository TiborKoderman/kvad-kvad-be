using Microsoft.AspNetCore.Mvc;
using Zeroconf;

[ApiController]
[Route("api/[controller]")]
public class MdnsController : ControllerBase
{
    private readonly MdnsDiscoveryService _mdnsDiscoveryService;

    public MdnsController(MdnsDiscoveryService mdnsDiscoveryService)
    {
        _mdnsDiscoveryService = mdnsDiscoveryService;
    }

    [HttpGet("devices")]
    public ActionResult<IReadOnlyList<IZeroconfHost>> GetMdnsDevices()
    {
        var devices = _mdnsDiscoveryService.ListMdnsDevices();
        return Ok(devices);
    }
}