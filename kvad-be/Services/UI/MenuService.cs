using kvad_be.Database;

public class MenuService(AppDbContext context)
{

  public Task<List<SidebarItem>> GetSidebarItems(User user)
  {
    return Task.FromResult(user.Sidebar);
  }

  public Task UpdateSidebarItems(User user, List<SidebarItem> newSidebar)
  {
    user.Sidebar = newSidebar;
    context.Users.Update(user);
    context.SaveChanges();
    return Task.CompletedTask;
  }

}