using Microsoft.EntityFrameworkCore;
using kvad_be.Database;

public class NodesService
{

    private readonly AppDbContext _context;

    public NodesService(AppDbContext context)
    {
        _context = context;
    }


    public Task AddNode(Node node)
    {
        _context.Nodes.Add(node);
        return _context.SaveChangesAsync();
    }

    public Task<List<Node>> GetNodes()
    {
        return _context.Nodes.ToListAsync();
    }

    public Task<Node?> GetNode(Guid id)
    {
        return _context.Nodes.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task UpdateNode(Node node)
    {
        _context.Nodes.Update(node);
        return _context.SaveChangesAsync();
    }

    public async Task DeleteNode(Guid id)
    {
        Node? node = await _context.Nodes.FirstOrDefaultAsync(x => x.Id == id);
        if (node != null)
        {
            _context.Nodes.Remove(node);
        }
        await _context.SaveChangesAsync();
    }

    public async Task AddObservedService(Guid nodeId, string serviceId)
    {
        Node? node = await _context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedServices.Add(serviceId);
        }
        await _context.SaveChangesAsync();
    }

    public async Task RemoveObservedService(Guid nodeId, string serviceId)
    {
        Node? node = await _context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedServices.Remove(serviceId);
        }
        await _context.SaveChangesAsync();
    }

    public async Task AddObservedContainer(Guid nodeId, string containerId)
    {
        Node? node = await _context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedContainers.Add(containerId);
        }
        await _context.SaveChangesAsync();
    }

    public async Task RemoveObservedContainer(Guid nodeId, string containerId)
    {
        Node? node = await _context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedContainers.Remove(containerId);
        }
        await _context.SaveChangesAsync();
    }





}