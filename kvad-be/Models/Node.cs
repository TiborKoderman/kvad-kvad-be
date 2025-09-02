public class Node
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    public List<string> observedServices { get; set; } = [];
    public List<string> observedContainers { get; set; } = [];

}