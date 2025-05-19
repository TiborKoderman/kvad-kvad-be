using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ScadaController : ControllerBase
{

    private readonly ScadaService _scadaService;

    public ScadaController(ScadaService scadaService)
    {
        _scadaService = scadaService;
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetAllScadaObjectTemplates()
    {
        var templates = await _scadaService.GetAllScadaObjectTemplates();
        return Ok(templates);
    }
    [HttpGet("template/{id}")]
    public async Task<IActionResult> GetScadaObjectTemplateById(Guid id)
    {
        var template = await _scadaService.GetScadaObjectTemplateById(id);
        if (template == null)
        {
            return NotFound("ScadaObjectTemplate not found.");
        }

        return Ok(template);
    }
    [HttpDelete("template/{id}")]
    public async Task<IActionResult> DeleteScadaObjectTemplate(Guid id)
    {
        await _scadaService.DeleteScadaObjectTemplate(id);
        return Ok();
    }
    [HttpPost("template")]
    public async Task<IActionResult> UpsertScadaObjectTemplate([FromBody] ScadaObjectTemplateDTO dto)
    {
        var template = await _scadaService.UpsertScadaObjectTemplate(dto);
        return CreatedAtAction(nameof(GetScadaObjectTemplateById), new { id = template.Id }, template);
    }

}