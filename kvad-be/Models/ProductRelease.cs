public class ProductRelease
{
    public Guid Id { get; set; }
    public string Version { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
}