
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

  public static readonly Header DataType = "dataType";
  public static readonly Header ContentType = "contentType";
  public static readonly Header ContentLength = "contentLength";
  public static readonly Header Type = "type";
  public static readonly Header Topic = "topic";
  public static readonly Header Error = "error";
  public static readonly Header Reason = "reason";
  public static readonly Header Id = "id";
  public static readonly Header ReplyTo = "replyTo";
  public static readonly Header CorrelationId = "correlationId";
  public static readonly Header Timestamp = "timestamp";
  public static readonly Header UserId = "userId";
  public static readonly Header From = "from";
  public static readonly Header To = "to";
  public static readonly Header Server = "server";
  public static readonly Header Status = "status";
  public static readonly Header Scope = "scope";
  public static readonly Header Delivered = "delivered";
}
