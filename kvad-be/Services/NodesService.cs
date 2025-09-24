using Microsoft.EntityFrameworkCore;
using kvad_be.Database;

public class NodesService(AppDbContext context)
{
    public Task AddNode(Node node)
    {
        context.Nodes.Add(node);
        return context.SaveChangesAsync();
    }

    public Task<List<Node>> GetNodes()
    {
        return context.Nodes.ToListAsync();
    }

    public Task<Node?> GetNode(Guid id)
    {
        return context.Nodes.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task UpdateNode(Node node)
    {
        context.Nodes.Update(node);
        return context.SaveChangesAsync();
    }

    public async Task DeleteNode(Guid id)
    {
        Node? node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == id);
        if (node != null)
        {
            context.Nodes.Remove(node);
        }
        await context.SaveChangesAsync();
    }

    public async Task AddObservedService(Guid nodeId, string serviceId)
    {
        Node? node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedServices.Add(serviceId);
        }
        await context.SaveChangesAsync();
    }

    public async Task RemoveObservedService(Guid nodeId, string serviceId)
    {
        Node? node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedServices.Remove(serviceId);
        }
        await context.SaveChangesAsync();
    }

    public async Task AddObservedContainer(Guid nodeId, string containerId)
    {
        Node? node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedContainers.Add(containerId);
        }
        await context.SaveChangesAsync();
    }

    public async Task RemoveObservedContainer(Guid nodeId, string containerId)
    {
        Node? node = await context.Nodes.FirstOrDefaultAsync(x => x.Id == nodeId);
        if (node != null)
        {
            node.observedContainers.Remove(containerId);
        }
        await context.SaveChangesAsync();
    }





}