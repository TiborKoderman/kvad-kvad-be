using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    private readonly AuthService _auth;

    public DashboardController(DashboardService dashboardService, AuthService authService)
    {
        _dashboardService = dashboardService;
        _auth = authService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetDashboard()
    {
                var user = await _auth.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(await _dashboardService.GetDashboards(user));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDashboard(Guid id)
    {
                var user = await _auth.GetUser(User);
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
                var user = await _auth.GetUser(User);
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
                var user = await _auth.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(await _dashboardService.SaveDashboard(user, dashboardDTO));
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetDashboardTypes()
    {
        return Ok(await _dashboardService.GetDashboardTypes());
    }
}