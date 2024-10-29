using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DockerController : Controller
{
    private DockerService _dockerService;

    public DockerController(DockerService dockerService)
    {
        _dockerService = dockerService;
    }

    [HttpGet("containers")]
    public async Task<IActionResult> GetContainers()
    {
        return Ok(await _dockerService.listDockerContainers());
    }

    [HttpGet("container/{containerId}/stop")]
    public async Task<IActionResult> StopContainer(String containerId)
    {
        return Ok(await _dockerService.stopContainer(containerId));
    }

    [HttpGet("container/{containerId}/start")]
    public async Task<IActionResult> StartContainer(String containerId)
    {
        return Ok(await _dockerService.startContainer(containerId));
    }

}