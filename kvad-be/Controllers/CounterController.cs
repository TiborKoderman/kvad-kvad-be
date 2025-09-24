using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CounterController(CounterService counterService) : Controller
{
    [HttpGet("increment")]
    public async Task<IActionResult> Increment()
    {
        return Ok(await counterService.Increment());
    }

    [HttpGet("decrement")]
    public async Task<IActionResult> Decrement()
    {
        return Ok(await counterService.Decrement());
    }

    [HttpGet("reset")]
    public async Task<IActionResult> Reset()
    {
        return Ok(await counterService.Reset());
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        return Ok(await counterService.GetCounter());
    }
}