public sealed class JwtOptions
{
  public string Key { get; set; } = default!;
  public string Issuer { get; set; } = default!;
  public string ValidIssuer { get; set; } = default!;
  public string[] ValidAudiences { get; set; } = [];
  public int LifetimeMinutes { get; set; }
  public string Audience { get; set; } = default!;
}