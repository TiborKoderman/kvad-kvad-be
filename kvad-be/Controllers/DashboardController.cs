using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetDashboard()
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(await _dashboardService.GetDashboards(user));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDashboard(Guid id)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        var dashboard = await _dashboardService.GetDashboard(user, id);
        if (dashboard == null)
        {
            return NotFound();
        }
        return Ok(dashboard);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDashboard(Guid id)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        await _dashboardService.DeleteDashboard(user, id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SaveDashboard(DashboardDTO dashboardDTO)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(await _dashboardService.SaveDashboard(user, dashboardDTO));
    }

    
}