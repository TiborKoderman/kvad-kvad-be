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
                Groups = user.PrivateGroup != null ? new List<Group> { user.PrivateGroup } : new List<Group>(),
                Icon = dashboardDTO.Icon,
                Color = dashboardDTO.Color,
            };
            dashboard.Layout = new Layout
            {
                Id = 0,
                Dashboard = dashboard,
                DashboardId = dashboard.Id,
                Direction = enumDirection.row,
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

    public async Task<Dashboard> AddLayout(User user, Guid DashboardId, enumDirection direction, int parentId)
    {

    var dashboard = await GetDashboard(user, DashboardId);
        if (dashboard == null)
        {
            throw new InvalidOperationException("Dashboard not found or access denied.");
        }

        int index = _context.Layouts
            .Where(l => l.DashboardId == DashboardId)
            .OrderByDescending(l => l.Id)
            .Select(l => l.Id)
            .FirstOrDefault() + 1;

        var layout = new Layout
        {
            Id = index,
            Dashboard = dashboard,
            DashboardId = dashboard.Id,
            Direction = direction,
            ParentId = parentId
        };

        _context.Layouts.Add(layout);
        await _context.SaveChangesAsync();
        return dashboard;
    }
    public async Task DeleteDashboard(User user, Guid id)
    {
        var dashboard = await GetDashboard(user, id);
        if (dashboard == null)
        {
            throw new InvalidOperationException("Dashboard not found or access denied.");
        }
        _context.Dashboards.Remove(dashboard);
        await _context.SaveChangesAsync();
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