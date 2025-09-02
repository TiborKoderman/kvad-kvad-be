using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{

    private readonly AuthService _authService;

    public RolesController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _authService.GetAllRoles();
        return Ok(roles);
    }

}