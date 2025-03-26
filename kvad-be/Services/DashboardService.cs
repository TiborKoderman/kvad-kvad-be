using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dashboard>> GetDashboards(User user)
    {
        return await _context.Dashboards
            .Where(d => d.Owner == user || d.Groups.Any(g => g.Users.Contains(user)))
            .ToListAsync();
    }

    public async Task<Dashboard> SaveDashboard(User user, DashboardDTO dashboardDTO)
    {
        Dashboard dashboard;
        if(dashboardDTO.Id == null)
        {
            dashboard = new Dashboard
            {
                Id = Guid.NewGuid(),
                Owner = user,
                Name = dashboardDTO.Name,
                Description = dashboardDTO.Description,
                Scrollable = dashboardDTO.Scrollable,
                Icon = dashboardDTO.Icon,
                Color = dashboardDTO.Color,
                Items = new List<DashboardItem>() // Initialize the required Items property
            };
            _context.Dashboards.Add(dashboard);
            await _context.SaveChangesAsync();
            return dashboard;
        }

        dashboard = await GetDashboard(user, dashboardDTO.Id.Value);
        if (dashboard == null)
        {
            throw new InvalidOperationException("Dashboard not found or access denied.");
        }
        dashboard.Name = dashboardDTO.Name;
        dashboard.Description = dashboardDTO.Description;
        dashboard.Scrollable = dashboardDTO.Scrollable;
        dashboard.Icon = dashboardDTO.Icon;
        dashboard.Color = dashboardDTO.Color;
        await _context.SaveChangesAsync();
        return dashboard;
    }

    public async Task<Dashboard> GetDashboard(User user, Guid id)
    {
        var dashboard = await _context.Dashboards
            .Where(d => d.Id == id && (d.Owner == user || d.Groups.Any(g => g.Users.Contains(user))))
            .FirstOrDefaultAsync();

        if (dashboard == null)
        {
            throw new InvalidOperationException("Dashboard not found or access denied.");
        }

        return dashboard;
    }
}