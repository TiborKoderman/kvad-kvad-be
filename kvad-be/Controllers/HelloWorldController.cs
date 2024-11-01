using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class HelloWorldController : Controller
{
    [HttpGet]
    public Task<IActionResult> HelloWorld()
    {
        return Task.FromResult<IActionResult>(Ok("Hello, World!"));
    }
}