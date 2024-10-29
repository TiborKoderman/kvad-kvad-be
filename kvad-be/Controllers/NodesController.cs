using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kvad_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly NodesService _nodesService;

        public NodesController(NodesService nodesService)
        {
            _nodesService = nodesService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNode([FromBody] Node node)
        {
            await _nodesService.AddNode(node);
            return CreatedAtAction(nameof(GetNode), new { id = node.Id }, node);
        }

        [HttpGet]
        public async Task<ActionResult<List<Node>>> GetNodes()
        {
            var nodes = await _nodesService.GetNodes();
            return Ok(nodes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Node>> GetNode(Guid id)
        {
            var node = await _nodesService.GetNode(id);
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

            await _nodesService.UpdateNode(node);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(Guid id)
        {
            await _nodesService.DeleteNode(id);
            return NoContent();
        }

        [HttpPost("{nodeId}/observedServices/{serviceId}")]
        public async Task<IActionResult> AddObservedService(Guid nodeId, string serviceId)
        {
            await _nodesService.AddObservedService(nodeId, serviceId);
            return NoContent();
        }

        [HttpDelete("{nodeId}/observedServices/{serviceId}")]
        public async Task<IActionResult> RemoveObservedService(Guid nodeId, string serviceId)
        {
            await _nodesService.RemoveObservedService(nodeId, serviceId);
            return NoContent();
        }

        [HttpPost("{nodeId}/observedContainers/{containerId}")]
        public async Task<IActionResult> AddObservedContainer(Guid nodeId, string containerId)
        {
            await _nodesService.AddObservedContainer(nodeId, containerId);
            return NoContent();
        }

        [HttpDelete("{nodeId}/observedContainers/{containerId}")]
        public async Task<IActionResult> RemoveObservedContainer(Guid nodeId, string containerId)
        {
            await _nodesService.RemoveObservedContainer(nodeId, containerId);
            return NoContent();
        }
    }
}