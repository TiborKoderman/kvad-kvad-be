
namespace kvad_be.Services.WebSocket;

public readonly struct Header : IEquatable<Header>
{
  private readonly string _value;

  private Header(string value)
  {
    _value = value;
  }

  public static implicit operator Header(string value) => new Header(value);
  public static implicit operator string(Header header) => header._value;

  public override string ToString() => _value;

  public override bool Equals(object? obj) => obj is Header other && Equals(other);

  public bool Equals(Header other) =>
    string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);

  public override int GetHashCode() =>
    _value?.ToLowerInvariant().GetHashCode() ?? 0;

  public static bool operator ==(Header left, Header right) => left.Equals(right);
  public static bool operator !=(Header left, Header right) => !left.Equals(right);

  public static readonly Header DataType = "Data-Type";
  public static readonly Header ContentType = "Content-Type";
  public static readonly Header ContentLength = "Content-Length";
  public static readonly Header Type = "Type";
  public static readonly Header Topic = "Topic";
  public static readonly Header Error = "Error";
  public static readonly Header Reason = "Reason";
  public static readonly Header Id = "Id";
  public static readonly Header ReplyTo = "Reply-To";
  public static readonly Header CorrelationId = "Correlation-Id";
  public static readonly Header Timestamp = "Timestamp";
  public static readonly Header UserId = "User-Id";
  public static readonly Header From = "From";
  public static readonly Header To = "To";
  public static readonly Header Server = "Server";
  public static readonly Header Status = "Status";
  public static readonly Header Scope = "Scope";
  public static readonly Header Delivered = "Delivered";
}
