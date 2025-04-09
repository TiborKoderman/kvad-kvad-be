using System.Text.Json.Serialization;

public class Group
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public List<User> Users { get; set; } = [];

    [JsonIgnore]
    public User? PrivateOwner { get; set; }
    [JsonIgnore]
    public List<Dashboard> Dashboards { get; set; } = [];
    [JsonIgnore]
    public List<Device> Devices { get; set; } = [];
}