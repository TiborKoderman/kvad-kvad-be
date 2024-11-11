public class Costumer
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}