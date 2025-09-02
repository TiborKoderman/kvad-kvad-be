using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using kvad_be.Database;

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

    public async Task DeleteScadaObjectTemplate(Guid id)
    {
        var template = await GetScadaObjectTemplateById(id);
        if (template != null)
        {
            _context.ScadaObjectTemplates.Remove(template);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ScadaObjectTemplate> UpsertScadaObjectTemplate(ScadaObjectTemplateDTO dto)
    {
        var template = dto.Id == null
            ? new ScadaObjectTemplate { Id = Guid.NewGuid(), Name = dto.Name, Data = dto.Data ?? JsonDocument.Parse("{}") }
            : await GetScadaObjectTemplateById(dto.Id.Value) ?? throw new Exception("ScadaObjectTemplate not found");

        template.Name = dto.Name ?? template.Name;
        template.Data = dto.Data ?? template.Data;

        if (dto.Id == null)
            await _context.ScadaObjectTemplates.AddAsync(template);
        else
            _context.ScadaObjectTemplates.Update(template);

        await _context.SaveChangesAsync();
        return template;
    }
}