public class ProductRelease
{
    public Guid Id { get; set; }
    public required string Version { get; set; }
    public DateTime ReleaseDate { get; set; }
    public required string Description { get; set; }
    public Guid ProductId { get; set; }
    public required Product Product { get; set; }
}