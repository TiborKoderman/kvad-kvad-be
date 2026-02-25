
namespace kvad_be.Models.User;

public class SidebarItem
{
  string Slug = "";
  string? Name = null!;
  Guid? DashboardId = null!;
  string? Icon = null!;
  List<SidebarItem>? Children = null!;

}
