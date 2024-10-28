public class ProductMatrix
{
    public Guid Id { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double Discount { get; set; }
    public double Total { get; set; }
}