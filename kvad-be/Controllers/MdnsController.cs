using Microsoft.AspNetCore.Mvc;
using Zeroconf;

[ApiController]
[Route("api/[controller]")]
public class MdnsController(MdnsService mdnsService) : ControllerBase
{
    [HttpGet("devices")]
    public ActionResult<IReadOnlyList<IZeroconfHost>> GetMdnsDevices()
    {
        var devices = mdnsService.ListMdnsDevices();
        return Ok(devices);
    }
}