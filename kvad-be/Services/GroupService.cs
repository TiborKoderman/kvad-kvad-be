using Microsoft.EntityFrameworkCore;

public class GroupService(AppDbContext context, ILogger<GroupService> logger)
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<GroupService> _logger = logger;

    public static bool IsMemberOfAnyGroup(User user, List<Group> groups)
    {
        foreach (var group in groups)
        {
            if (group.Users.Contains(user))
            {
                return true;
            }
        }
        return false;
    }

    public async Task<List<Group>> GetGroups(User user)
    {
        return await _context.Groups
            .Where(g => g.Users.Contains(user))
            .ToListAsync();
    }

    internal bool IsMemberOfAnyGroup(User user, Group[] groups)
    {
        throw new NotImplementedException();
    }
}