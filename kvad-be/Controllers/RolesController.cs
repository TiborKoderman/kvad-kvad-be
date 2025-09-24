using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RolesController(AuthService authService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await authService.GetAllRoles();
        return Ok(roles);
    }

}