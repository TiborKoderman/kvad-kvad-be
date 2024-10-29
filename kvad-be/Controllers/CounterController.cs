using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CounterController : Controller
{
    private readonly CounterService _counterService;

    public CounterController(CounterService counterService)
    {
        _counterService = counterService;
    }

    [HttpGet("increment")]
    public async Task<IActionResult> Increment()
    {
        return Ok(await _counterService.Increment());
    }

    [HttpGet("decrement")]
    public async Task<IActionResult> Decrement()
    {
        return Ok(await _counterService.Decrement());
    }

    [HttpGet("reset")]
    public async Task<IActionResult> Reset()
    {
        return Ok(await _counterService.Reset());
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        return Ok(await _counterService.GetCounter());
    }
}