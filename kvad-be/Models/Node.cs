public class Node {
    public Guid Id { get; set; }
    public string Name { get; set; }

    public List<string> observedServices { get; set; }
    public List<string> observedContainers { get; set; }

}