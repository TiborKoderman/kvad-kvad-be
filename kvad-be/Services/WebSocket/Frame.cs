namespace kvad_be.Services.WebSocket;

using System.Net.WebSockets;
using System.Text.Json;

internal sealed class Frame
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
    Headers[Header.DataType] = "text";
    Payload = Encoding.UTF8.GetBytes(textPayload);
  }

  // Constructor for JSON payload (with generic struct/class)
  public Frame(Command command, object jsonObject, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers[Header.DataType] = "json";
    Headers[Header.ContentType] = "application/json";

    var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(jsonObject);
    Payload = jsonBytes;
  }

  // Constructor for binary payload
  public Frame(Command command, byte[] binaryData, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers[Header.DataType] = "binary";
    Headers[Header.ContentLength] = $"{binaryData.Length}";
    Payload = binaryData;
  }

  // Constructor for ReadOnlyMemory<byte> (zero-copy)
  public Frame(Command command, ReadOnlyMemory<byte> binaryData, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers[Header.DataType] = "binary";
    Headers[Header.ContentLength] = $"{binaryData.Length}";
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
    frame.Headers[Header.DataType] = "json";
    frame.Headers[Header.ContentType] = "application/json";
    frame.Headers[Header.Type] = typeof(T).Name;

    frame.Payload = JsonSerializer.SerializeToUtf8Bytes(data);
    return frame;
  }



  public static Frame Parse(ReadOnlyMemory<byte> data)
  {
    var span = data.Span;
    var frame = new Frame();

    int position = -1;
    int lineStart = 0;
    bool inHeaders = true;
    bool firstLine = true;

    void ProcessLine(ReadOnlySpan<byte> line)
    {
      // Tolerate a single trailing '\r' even though we frame with '\n'
      if (line.Length > 0 && line[^1] == (byte)'\r')
        line = line[..^1];

      if (firstLine)
      {
        var commandStr = Encoding.UTF8.GetString(line);

        // If you have an enum Command:
        if (!Enum.TryParse<Command>(commandStr, ignoreCase: true, out var cmd))
          throw new InvalidOperationException($"Unknown command: {commandStr}");
        frame.Command = cmd; // <-- if your Frame.Command is string, set frame.Command = commandStr;

        firstLine = false;
        return;
      }

      if (inHeaders)
      {
        if (line.Length == 0)
        {
          // Blank line = end of headers; payload starts at next byte
          inHeaders = false;
          return;
        }

        int colon = line.IndexOf((byte)':');
        if (colon > 0)
        {
          var key = Encoding.UTF8.GetString(line[..colon]).Trim();
          var val = Encoding.UTF8.GetString(line[(colon + 1)..]).Trim();
          if (key.Length != 0) frame.Headers[key] = val;
        }
        // else: ignore malformed header line without colon
      }
    }

    for (int i = 0; i < span.Length; i++)
    {
      if (span[i] == (byte)'\n')
      {
        var line = span.Slice(lineStart, i - lineStart);
        ProcessLine(line);

        if (!inHeaders && position < 0)
        {
          // First time we exit headers: payload starts after this '\n'
          position = i + 1;
          // Do NOT break—there might be payload bytes after this; we only needed the index.
          // We’ll stop scanning lines here to avoid misinterpreting payload contents as lines.
          break;
        }

        lineStart = i + 1;
      }
    }

    // If we never hit a '\n' for the last line (EOF without newline), process the trailing line
    if (position < 0) // only if we haven’t already determined start-of-payload
    {
      if (lineStart < span.Length)
      {
        var tailLine = span.Slice(lineStart);
        ProcessLine(tailLine);
      }

      // If headers are still "open", there was no blank line → no payload
      if (inHeaders)
      {
        position = span.Length; // payload empty
      }
      else if (position < 0)
      {
        // We ended headers in the EOF tail-line case; payload begins after that line.
        // Since there was no '\n' to step past, payload starts exactly at end of tail line.
        position = span.Length;
      }
    }

    // Slice payload zero-copy
    if (position < 0) position = span.Length; // safety
    if (position <= span.Length)
      frame.Payload = data[position..];
    else
      frame.Payload = ReadOnlyMemory<byte>.Empty;

    // Respect ContentLength (if present and sane)
    if (frame.Headers.TryGetValue(Header.ContentLength, out var lenStr) &&
        int.TryParse(lenStr, out var contentLen) &&
        contentLen >= 0)
    {
      if (contentLen > frame.Payload.Length)
        throw new InvalidOperationException($"Declared ContentLength {contentLen} exceeds available payload {frame.Payload.Length}.");
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
    var dt = GetHeader(Header.DataType);
    return string.Equals(dt, "binary", StringComparison.OrdinalIgnoreCase)
      ? WebSocketMessageType.Binary
      : WebSocketMessageType.Text;
  }

}

internal enum Command
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

internal enum DataType
{
  Text,
  Json,
  Binary
}



internal record Status(int Code, string Description)
{
  public static readonly Status OK = new(200, "OK");
  public static readonly Status BadRequest = new(400, "Bad Request");
  public static readonly Status Unauthorized = new(401, "Unauthorized");
  public static readonly Status Forbidden = new(403, "Forbidden");
  public static readonly Status NotFound = new(404, "Not Found");
  public static readonly Status Conflict = new(409, "Conflict");
  public static readonly Status InternalServerError = new(500, "Internal Server Error");
  public static readonly Status ServiceUnavailable = new(503, "Service Unavailable");


  public override string ToString() => $"{Code} {Description}";

  public static Status FromCode(int code) => code switch
  {
    200 => OK,
    400 => BadRequest,
    401 => Unauthorized,
    403 => Forbidden,
    404 => NotFound,
    409 => Conflict,
    500 => InternalServerError,
    503 => ServiceUnavailable,
    _ => new Status(code, "Unknown")
  };

  //implicit string conversion
  public static implicit operator string(Status status) => status.ToString();


  //implicit conversion to key-value pair for headers
  public static implicit operator (string, string)(Status status) => ("status", status.ToString());
}



internal enum Headers
{
  DataType,
  ContentType,
  ContentLength,
  Type,
  Destination,
  Topic,
  Id,
  Receipt,
  ReceiptId,
  Authorization,
  UserId,
  Timestamp
}