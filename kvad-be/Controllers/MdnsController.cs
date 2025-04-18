using Microsoft.AspNetCore.Mvc;
using Zeroconf;

[ApiController]
[Route("api/[controller]")]
public class MdnsController : ControllerBase
{
    private readonly MdnsService _mdnsService;

    public MdnsController(MdnsService mdnsService)
    {
        _mdnsService = mdnsService;
    }

    [HttpGet("devices")]
    public ActionResult<IReadOnlyList<IZeroconfHost>> GetMdnsDevices()
    {
        var devices = _mdnsService.ListMdnsDevices();
        return Ok(devices);
    }
}