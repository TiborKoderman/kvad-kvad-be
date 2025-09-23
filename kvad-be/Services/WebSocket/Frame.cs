using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class Frame
{
  public Command Command { get; set; }
  public Dictionary<string, string> Headers { get; set; } = new(StringComparer.Ordinal);
  public ReadOnlyMemory<byte> Payload { get; set; } = ReadOnlyMemory<byte>.Empty;

  public Frame() { }


  // Constructor for text payload
  public Frame(Command command, string textPayload, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "text";
    Payload = Encoding.UTF8.GetBytes(textPayload);
  }

  // Constructor for JSON payload (with generic struct/class)
  public Frame(Command command, object jsonObject, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "json";
    Headers["ContentType"] = "application/json";

    var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(jsonObject);
    Payload = jsonBytes;
  }

  // Constructor for binary payload
  public Frame(Command command, byte[] binaryData, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "binary";
    Headers["ContentLength"] = binaryData.Length.ToString();
    Payload = binaryData;
  }

  // Constructor for ReadOnlyMemory<byte> (zero-copy)
  public Frame(Command command, ReadOnlyMemory<byte> binaryData, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "binary";
    Headers["ContentLength"] = binaryData.Length.ToString();
    Payload = binaryData;
  }


  // Frame with no payload
  public Frame(Command command, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
  }

  // Static factory method for creating typed frames
  public static Frame CreateJson<T>(Command command, T data, Dictionary<string, string>? headers = null) where T : notnull
  {
    var frame = new Frame
    {
      Command = command,
      Headers = headers ?? []
    };
    frame.Headers["DataType"] = "json";
    frame.Headers["ContentType"] = "application/json";
    frame.Headers["Type"] = typeof(T).Name;

    frame.Payload = JsonSerializer.SerializeToUtf8Bytes(data);
    return frame;
  }


  public static Frame Parse(ReadOnlyMemory<byte> data)
  {
    var span = data.Span;
    var frame = new Frame();

    int position = 0;
    int lineStart = 0;
    bool inHeaders = true;
    bool firstLine = true;

    for (int i = 0; i < span.Length; i++)
    {
      if (span[i] == (byte)'\n')
      {
        int lineLength = i - lineStart;

        if (firstLine)
        {
          var commandStr = Encoding.UTF8.GetString(span.Slice(lineStart, lineLength));
          if (!Enum.TryParse<Command>(commandStr, out var command))
            throw new InvalidOperationException($"Unknown command: {commandStr}");
          frame.Command = command;
          firstLine = false;
        }
        else if (inHeaders)
        {
          if (lineLength == 0)
          {
            // Empty line marks end of headers
            inHeaders = false;
            position = i + 1;
            break;
          }

          var lineSpan = span.Slice(lineStart, lineLength);
          var colonIndex = lineSpan.IndexOf((byte)':');

          if (colonIndex > 0)
          {
            var key = Encoding.UTF8.GetString(lineSpan[..colonIndex]).Trim();
            var value = Encoding.UTF8.GetString(lineSpan[(colonIndex + 1)..]).Trim();
            frame.Headers[key] = value;
          }
        }

        lineStart = i + 1;
      }
    }

    // Extract payload if present (zero-copy)
    if (position < span.Length)
    {
      frame.Payload = data[position..];
    }

    if (frame.Headers.TryGetValue("ContentLength", out var lenStr) &&
    int.TryParse(lenStr, out var contentLen) &&
    contentLen >= 0 &&
    contentLen <= frame.Payload.Length)
    {
      frame.Payload = frame.Payload[..contentLen];
    }


    return frame;
  }


  // Zero-copy serialization to ArraySegment
  public ArraySegment<byte> ToArraySegment()
  {
    // Calculate size
    int headerSize = CalculateHeaderSize();
    int totalSize = headerSize + Payload.Length;

    var buffer = new byte[totalSize];
    int position = 0;

    // Write command

    // COMMAND + LF
    position += Encoding.UTF8.GetBytes(Command.ToString(), buffer.AsSpan(position));
    buffer[position++] = (byte)'\n';

    // Headers
    foreach (var kvp in Headers)
    {
      position += Encoding.UTF8.GetBytes(kvp.Key, buffer.AsSpan(position));
      buffer[position++] = (byte)':';
      buffer[position++] = (byte)' ';
      position += Encoding.UTF8.GetBytes(kvp.Value, buffer.AsSpan(position));
      buffer[position++] = (byte)'\n';
    }

    // Empty line between headers and payload
    buffer[position++] = (byte)'\n';

    // Copy payload
    if (!Payload.IsEmpty)
    {
      Payload.CopyTo(new Memory<byte>(buffer, position, Payload.Length));
    }

    return new ArraySegment<byte>(buffer, 0, totalSize);
  }

  // Helper to calculate header size
  private int CalculateHeaderSize()
  {
    int size = Encoding.UTF8.GetByteCount(Command.ToString()) + 1; // LF
    foreach (var kvp in Headers)
      size += Encoding.UTF8.GetByteCount(kvp.Key) + 2 + Encoding.UTF8.GetByteCount(kvp.Value) + 2; // "Key: Value\n"
    size += 1;   // empty line
    return size;
  }
  // Deserialize JSON payload to type T
  public T? GetJsonPayload<T>() where T : class
  {
    if (Payload.IsEmpty) return null;
    return JsonSerializer.Deserialize<T>(Payload.Span);
  }

  // Get text payload
  public string GetTextPayload()
  {
    if (Payload.IsEmpty) return string.Empty;
    return Encoding.UTF8.GetString(Payload.Span);
  }

  // Get binary payload as byte array
  public byte[] GetBinaryPayload()
  {
    return Payload.ToArray();
  }


  public string? GetHeader(string key)
    => Headers.TryGetValue(key, out var v) ? v : null;
  public void SetHeader(string key, string value) => Headers[key] = value;

  public WebSocketMessageType GetSuggestedMessageType()
  {
    var dt = GetHeader("DataType");
    return string.Equals(dt, "binary", StringComparison.OrdinalIgnoreCase)
      ? WebSocketMessageType.Binary
      : WebSocketMessageType.Text;
  }

}

public enum Command
{
  CONNECT,
  CONNECTED,
  DISCONNECT,

  SUBSCRIBE,
  SUBSCRIBED,
  UNSUBSCRIBE,
  UNSUBSCRIBED,

  SEND,
  PUBLISH,
  PUBLISHED,
  MESSAGE,
  ERROR,


  CREATE_TOPIC,
  DELETE_TOPIC,

  BEGIN,
  COMMIT,
  ABORT,
  ACK,
  NACK,
  RECEIPT,

  PING,
  PONG,
}

public enum DataType
{
  Text,
  Json,
  Binary
}



public record StatusCode(int Code, string Description)
{
  public static readonly StatusCode OK = new(200, "OK");
  public static readonly StatusCode BadRequest = new(400, "Bad Request");
  public static readonly StatusCode Unauthorized = new(401, "Unauthorized");
  public static readonly StatusCode Forbidden = new(403, "Forbidden");
  public static readonly StatusCode NotFound = new(404, "Not Found");
  public static readonly StatusCode Conflict = new(409, "Conflict");
  public static readonly StatusCode InternalServerError = new(500, "Internal Server Error");
  public static readonly StatusCode ServiceUnavailable = new(503, "Service Unavailable");


  public override string ToString() => $"{Code} {Description}";

  public static StatusCode FromCode(int code) => code switch
  {
    200 => OK,
    400 => BadRequest,
    401 => Unauthorized,
    403 => Forbidden,
    404 => NotFound,
    409 => Conflict,
    500 => InternalServerError,
    503 => ServiceUnavailable,
    _ => new StatusCode(code, "Unknown")
  };

  //implicit string conversion
  public static implicit operator string(StatusCode status) => status.ToString();


  //implicit conversion to key-value pair for headers
  public static implicit operator (string, string)(StatusCode status) => ("status", status.ToString());
}
