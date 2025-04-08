public class ProductMatrix
{
    public Guid Id { get; set; }
    public required Product Product { get; set; }
    public required int Quantity { get; set; }
    public required double Price { get; set; }
    public required double Discount { get; set; }
    public required double Total { get; set; }
}