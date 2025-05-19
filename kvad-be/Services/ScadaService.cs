using Microsoft.EntityFrameworkCore;

public class ScadaService
{
    private readonly AppDbContext _context;

    public ScadaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScadaObjectTemplate>> GetAllScadaObjectTemplates()
    {
        return await _context.ScadaObjectTemplates.ToListAsync();
    }

    public async Task<ScadaObjectTemplate?> GetScadaObjectTemplateById(Guid id)
    {
        return await _context.ScadaObjectTemplates.FindAsync(id);
    }

}