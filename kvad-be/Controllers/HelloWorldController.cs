using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : Controller
{
    [HttpGet]
    public Task<IActionResult> HelloWorld()
    {
        return Task.FromResult<IActionResult>(Ok("Hello, World!"));
    }
}