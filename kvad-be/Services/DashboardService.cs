using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using kvad_be.Database;

public class DashboardService(AppDbContext context, ILogger<DashboardService> logger)
{
    public Task<List<Dashboard>> GetDashboards(User user)
    {
        var dashboards = user.Groups
            .SelectMany(g => g.Dashboards)
            .Distinct()
            .ToList();

        return Task.FromResult(dashboards);
    }

    public async Task<Dashboard> SaveDashboard(User user, DashboardDTO dashboardDTO)
    {
        if (dashboardDTO == null)
        {
            logger.LogError("DashboardDTO is null.");
            throw new ArgumentNullException(nameof(dashboardDTO), "DashboardDTO cannot be null.");
        }

        if (string.IsNullOrEmpty(dashboardDTO.Name))
        {
            logger.LogError("DashboardDTO Name is null or empty.");
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
                TypeId = dashboardDTO.Type,
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
                context.Dashboards.Add(dashboard);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving new dashboard.");
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
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating dashboard.");
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

        int index = context.Layouts
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

        context.Layouts.Add(layout);
        await context.SaveChangesAsync();
        return dashboard;
    }

    public async Task DeleteDashboard(User user, Guid id)
    {
        var dashboard = await GetDashboard(user, id);
        if (dashboard == null)
        {
            throw new InvalidOperationException("Dashboard not found or access denied.");
        }

        context.Dashboards.Remove(dashboard);
        await context.SaveChangesAsync();
    }

    public async Task<Dashboard> GetDashboard(User user, Guid id)
    {
        var dashboard = await context.Dashboards
            .Where(d => d.Id == id && (d.Owner == user || d.Groups.Any(g => g.Users.Contains(user))))
            .FirstOrDefaultAsync();

        if (dashboard == null)
        {
            throw new InvalidOperationException("Dashboard not found or access denied.");
        }

        return dashboard;
    }

    public async Task<List<DashboardType>> GetDashboardTypes()
    {
        return await context.DashboardTypes
            .ToListAsync();
    }
}