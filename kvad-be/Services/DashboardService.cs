using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class DashboardService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(AppDbContext context, ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<List<Dashboard>> GetDashboards(User user)
    {
        var userId = user.Id;

        // Fetch dashboards directly with a single query
        var dashboards = await _context.Dashboards
            .Where(d => d.Owner.Id == userId || d.Groups.Any(g => g.Users.Any(u => u.Id == userId)))
            .Distinct()
            .ToListAsync();

        return dashboards;
    }

    public async Task<Dashboard> SaveDashboard(User user, DashboardDTO dashboardDTO)
    {
        if (dashboardDTO == null)
        {
            _logger.LogError("DashboardDTO is null.");
            throw new ArgumentNullException(nameof(dashboardDTO), "DashboardDTO cannot be null.");
        }

        if (string.IsNullOrEmpty(dashboardDTO.Name))
        {
            _logger.LogError("DashboardDTO Name is null or empty.");
            throw new ArgumentException("DashboardDTO Name cannot be null or empty.", nameof(dashboardDTO.Name));
        }

        Dashboard dashboard;
        if (dashboardDTO.Id == null)
        {
            dashboard = new Dashboard
            {
                Id = Guid.NewGuid(),
                Owner = user,
                Name = dashboardDTO.Name,
                Description = dashboardDTO.Description,
                Scrollable = dashboardDTO.Scrollable,
                Groups = [user.PrivateGroup],
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

            try
            {
                _context.Dashboards.Add(dashboard);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving new dashboard.");
                throw;
            }

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

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dashboard.");
            throw;
        }

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