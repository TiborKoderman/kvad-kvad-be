public class HistoricizationInterval{
    public required int Id { get; set; }
    public required string Name { get; set; } = "";
    public required TimeSpan? Interval { get; set; }
}