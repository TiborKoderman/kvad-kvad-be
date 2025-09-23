using System.Text;
using System.Text.Json;

public sealed class Frame
{
  public string Command { get; set; } = "";
  public Dictionary<string, string> Headers { get; set; } = new(StringComparer.Ordinal);
  public ReadOnlyMemory<byte> Payload { get; set; } = ReadOnlyMemory<byte>.Empty;

  public Frame() { }


  // Constructor for text payload
  public Frame(string command, string textPayload, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "text";
    Payload = Encoding.UTF8.GetBytes(textPayload);
  }

  // Constructor for JSON payload (with generic struct/class)
  public Frame(string command, object jsonObject, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "json";
    Headers["ContentType"] = "application/json";

    var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(jsonObject);
    Payload = jsonBytes;
  }

  // Constructor for binary payload
  public Frame(string command, byte[] binaryData, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "binary";
    Headers["ContentLength"] = binaryData.Length.ToString();
    Payload = binaryData;
  }

  // Constructor for ReadOnlyMemory<byte> (zero-copy)
  public Frame(string command, ReadOnlyMemory<byte> binaryData, Dictionary<string, string>? headers = null)
  {
    Command = command;
    Headers = headers ?? [];
    Headers["DataType"] = "binary";
    Headers["ContentLength"] = binaryData.Length.ToString();
    Payload = binaryData;
  }

  // Static factory method for creating typed frames
  public static Frame CreateJson<T>(string command, T data, Dictionary<string, string>? headers = null) where T : notnull
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
      if (span[i] == '\n')
      {
        var lineLength = i - lineStart - (i > 0 && span[i - 1] == '\r' ? 1 : 0);

        if (firstLine)
        {
          frame.Command = Encoding.UTF8.GetString(span.Slice(lineStart, lineLength));
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
            var key = Encoding.UTF8.GetString(lineSpan.Slice(0, colonIndex)).Trim();
            var value = Encoding.UTF8.GetString(lineSpan.Slice(colonIndex + 1)).Trim();
            frame.Headers[key] = value;
          }
        }

        lineStart = i + 1;
      }
    }

    // Extract payload if present (zero-copy)
    if (position < span.Length)
    {
      frame.Payload = data.Slice(position);
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

    // COMMAND + CRLF
    position += Encoding.UTF8.GetBytes(Command, buffer.AsSpan(position));
    buffer[position++] = (byte)'\r';
    buffer[position++] = (byte)'\n';

    // Headers
    foreach (var kvp in Headers)
    {
      position += Encoding.UTF8.GetBytes(kvp.Key, buffer.AsSpan(position));
      buffer[position++] = (byte)':';
      buffer[position++] = (byte)' ';
      position += Encoding.UTF8.GetBytes(kvp.Value, buffer.AsSpan(position));
      buffer[position++] = (byte)'\r';
      buffer[position++] = (byte)'\n';
    }

    // Empty line between headers and payload
    buffer[position++] = (byte)'\r';
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
    int size = Encoding.UTF8.GetByteCount(Command) + 2; // CRLF
    foreach (var kvp in Headers)
      size += Encoding.UTF8.GetByteCount(kvp.Key) + 2 + Encoding.UTF8.GetByteCount(kvp.Value) + 2; // "Key: Value\r\n"
    size += 2; // empty line
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


  public void SetHeader(string key, string value) => Headers[key] = value;
    

}

