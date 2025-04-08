public class MenuService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MenuService> _logger;

    public MenuService(AppDbContext context, ILogger<MenuService> logger)
    {
        _context = context;
        _logger = logger;
    }


}