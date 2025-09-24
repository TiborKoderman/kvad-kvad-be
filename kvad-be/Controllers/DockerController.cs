using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DockerController(DockerService dockerService) : ControllerBase
{
    [HttpGet("containers")]
    public async Task<IActionResult> GetContainers()
    {
        return Ok(await dockerService.listDockerContainers());
    }

    [HttpGet("container/{containerId}/stop")]
    public async Task<IActionResult> StopContainer(String containerId)
    {
        if (await dockerService.stopContainer(containerId) == 0)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpGet("container/{containerId}/start")]
    public async Task<IActionResult> StartContainer(String containerId)
    {
        if (await dockerService.startContainer(containerId) == 0)
        {
            return Ok();
        }
        return BadRequest();
    }

}