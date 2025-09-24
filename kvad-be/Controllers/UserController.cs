using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController(UserService userService, AuthService authService) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await userService.getUser(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await userService.getUser(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userService.getUsers();
        return Ok(users);
    }

    [HttpGet("table")]
    public async Task<IActionResult> GetUserTable()
    {
        var userTable = await userService.getUserTable();
        return Ok(userTable);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        await userService.updateUser(user);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] User user)
    {
        await userService.deleteUser(user);
        return Ok();
    }


    //upload icon
    [HttpPost("icon")]
    public async Task<IActionResult> UploadIcon(IFormFile icon)
    {
        var user = await userService.getUser("admin");
        if (user == null)
        {
            return NotFound();
        }
        await userService.uploadIcon(user.Id, icon);
        return Ok();
    }

    [HttpGet("icon/{username}")]
    public async Task<IActionResult> GetIcon(string username)
    {
        User? user = await userService.getUser(username);
        if (user == null)
        {
            return NotFound();
        }
        var icon = await userService.getIcon(user.Id);
        if (icon == null)
        {
            return NotFound();
        }
        return File(icon, "image/png");
    }
}