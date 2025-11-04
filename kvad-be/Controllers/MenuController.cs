using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kvad_be.Models.User;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MenuController(MenuService menuService) : Controller
{
  [HttpPost("sidebar")]
  public async Task<IActionResult> UpdateSidebar([FromServices] User user, [FromBody] List<SidebarItem> newSidebar)
  {
    await menuService.UpdateSidebarItems(user, newSidebar);
    return Ok(newSidebar);
  }
}