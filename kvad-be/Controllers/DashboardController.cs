using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(DashboardService dashboardService, AuthService authService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetDashboard()
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(await dashboardService.GetDashboards(user));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDashboard(Guid id)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var dashboard = await dashboardService.GetDashboard(user, id);
        if (dashboard == null)
        {
            return NotFound();
        }
        return Ok(dashboard);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDashboard(Guid id)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        await dashboardService.DeleteDashboard(user, id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SaveDashboard(DashboardDTO dashboardDTO)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(await dashboardService.SaveDashboard(user, dashboardDTO));
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetDashboardTypes()
    {
        return Ok(await dashboardService.GetDashboardTypes());
    }
}