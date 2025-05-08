public class HistoricizationInterval{
    public required int Id { get; set; }
    public required string Name { get; set; } = "";
    public TimeSpan? Interval { get; set; }
    public string? CronExpression { get; set; }
}