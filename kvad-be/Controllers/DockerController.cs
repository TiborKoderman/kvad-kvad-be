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
}