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
}