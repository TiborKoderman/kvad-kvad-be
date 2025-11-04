public class SidebarItem
{
  public required string Name { get; set; }
  public required string Icon { get; set; }
  public required string Link { get; set; }
  public List<SidebarItem>? Children { get; set; }
}