using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await _userService.getUser(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.getUser(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.getUsers();
        return Ok(users);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        await _userService.updateUser(user);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] User user)
    {
        await _userService.deleteUser(user);
        return Ok();
    }


    //upload icon
    [HttpPost("icon")]
    public async Task<IActionResult> UploadIcon(IFormFile icon)
    {
        var user = await _userService.getUser("admin");
        if (user == null)
        {
            return NotFound();
        }
        await _userService.uploadIcon(user.Id, icon);
        return Ok();
    }

    [HttpGet("icon/{username}")]
    public async Task<IActionResult> GetIcon(string username)
    {
        var user = await _userService.getUser(username);
        if (user == null)
        {
            return NotFound();
        }
        if (user == null)
        {
            return NotFound();
        }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var icon = await _userService.getIcon(user.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (icon == null)
        {
            return NotFound();
        }
        return File(icon, "image/png");
    }
}