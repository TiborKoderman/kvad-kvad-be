using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NodesController(NodesService nodesService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddNode([FromBody] Node node)
    {
        await nodesService.AddNode(node);
        return CreatedAtAction(nameof(GetNode), new { id = node.Id }, node);
    }

    [HttpGet]
    public async Task<ActionResult<List<Node>>> GetNodes()
    {
        var nodes = await nodesService.GetNodes();
        return Ok(nodes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Node>> GetNode(Guid id)
    {
        var node = await nodesService.GetNode(id);
        if (node == null)
        {
            return NotFound();
        }
        return Ok(node);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNode(Guid id, [FromBody] Node node)
    {
        if (id != node.Id)
        {
            return BadRequest();
        }

        await nodesService.UpdateNode(node);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNode(Guid id)
    {
        await nodesService.DeleteNode(id);
        return NoContent();
    }

    [HttpPost("{nodeId}/observedServices/{serviceId}")]
    public async Task<IActionResult> AddObservedService(Guid nodeId, string serviceId)
    {
        await nodesService.AddObservedService(nodeId, serviceId);
        return NoContent();
    }

    [HttpDelete("{nodeId}/observedServices/{serviceId}")]
    public async Task<IActionResult> RemoveObservedService(Guid nodeId, string serviceId)
    {
        await nodesService.RemoveObservedService(nodeId, serviceId);
        return NoContent();
    }

    [HttpPost("{nodeId}/observedContainers/{containerId}")]
    public async Task<IActionResult> AddObservedContainer(Guid nodeId, string containerId)
    {
        await nodesService.AddObservedContainer(nodeId, containerId);
        return NoContent();
    }

    [HttpDelete("{nodeId}/observedContainers/{containerId}")]
    public async Task<IActionResult> RemoveObservedContainer(Guid nodeId, string containerId)
    {
        await nodesService.RemoveObservedContainer(nodeId, containerId);
        return NoContent();
    }
}
