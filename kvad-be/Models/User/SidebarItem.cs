namespace kvad_be.Models.User;

public record SidebarItem(
  string Name,
  string Route,
  string Icon,
  List<SidebarItem>? Children
);