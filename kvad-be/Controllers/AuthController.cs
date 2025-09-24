using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace kvad_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDTO loginRequest)
    {
        if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            return BadRequest("Invalid login request");
        }
        var token = await authService.Authenticate(loginRequest.Username, loginRequest.Password);
        if (token == null)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDTO registerRequest)
    {
        if (registerRequest == null || string.IsNullOrWhiteSpace(registerRequest.Username) || string.IsNullOrWhiteSpace(registerRequest.Password))
        {
            return BadRequest("Invalid register request");
        }
        var user = await authService.Register(registerRequest.Username, registerRequest.Password);
        if (user == null)
        {
            return BadRequest("User already exists");
        }

        return Ok(user);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await authService.GetUsers();
        if (users == null)
        {
            return NotFound();
        }

        return Ok(users);
    }
}
