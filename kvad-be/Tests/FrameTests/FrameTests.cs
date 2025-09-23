using System.Text;
using System.Text.Json;
using Xunit;

// Tests for the Frame class in Services/WebSocket
public class FrameTests
{
    [Fact]
    public void TextFrame_SerializeAndParse_RoundTrip()
    {
        var original = new Frame("MSG", "hello world", new Dictionary<string, string> { { "Foo", "Bar" } });

        var seg = original.ToArraySegment();
        byte[]? arr;
        // Print serialized frame as UTF-8 text for inspection
        Console.WriteLine("--- Text Frame Raw ---");
        arr = seg.Array;
        if (arr != null)
        {
            Console.WriteLine(Encoding.UTF8.GetString(arr, seg.Offset, seg.Count));
        }
        else
        {
            Console.WriteLine("<null array>");
        }
        Console.WriteLine("----------------------");
        var parsed = Frame.Parse(new ReadOnlyMemory<byte>(seg.Array, seg.Offset, seg.Count));

        Assert.Equal(original.Command, parsed.Command);
        Assert.Equal("text", parsed.Headers["DataType"]);
        Assert.Equal("Bar", parsed.Headers["Foo"]);
        Assert.Equal(original.GetTextPayload(), parsed.GetTextPayload());
    }

    [Fact]
    public void JsonFrame_CreateJsonAndParse_RoundTrip()
    {
        var payload = new { Name = "Alice", Age = 30 };
        var original = Frame.CreateJson("UPDATE", payload);

        var seg = original.ToArraySegment();
        byte[]? arr;
        // Print serialized JSON frame as UTF-8 text
        Console.WriteLine("--- JSON Frame Raw ---");
        arr = seg.Array;
        if (arr != null)
        {
            Console.WriteLine(Encoding.UTF8.GetString(arr, seg.Offset, seg.Count));
        }
        else
        {
            Console.WriteLine("<null array>");
        }
        Console.WriteLine("----------------------");
        var parsed = Frame.Parse(new ReadOnlyMemory<byte>(seg.Array, seg.Offset, seg.Count));

        Assert.Equal("UPDATE", parsed.Command);
        Assert.Equal("json", parsed.Headers["DataType"]);
        Assert.Equal("application/json", parsed.Headers["ContentType"]);

        var parsedObj = JsonSerializer.Deserialize<JsonElement>(parsed.Payload.Span);
        Assert.Equal("Alice", parsedObj.GetProperty("Name").GetString());
        Assert.Equal(30, parsedObj.GetProperty("Age").GetInt32());
    }

    [Fact]
    public void BinaryFrame_SerializeAndParse_RoundTrip()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var original = new Frame("BIN", data, new Dictionary<string, string> { { "X", "Y" } });

        var seg = original.ToArraySegment();
        // Print serialized binary frame as header text + payload as hex
        Console.WriteLine("--- Binary Frame Raw ---");
        var raw = seg.Array;
        if (raw != null)
        {
            Console.WriteLine(Encoding.UTF8.GetString(raw, seg.Offset, seg.Count));
        }
        else
        {
            Console.WriteLine("<null array>");
        }
        Console.WriteLine("Payload (hex): " + BitConverter.ToString(original.GetBinaryPayload()));
        Console.WriteLine("------------------------");
        var parsed = Frame.Parse(new ReadOnlyMemory<byte>(seg.Array, seg.Offset, seg.Count));

        Assert.Equal("BIN", parsed.Command);
        Assert.Equal("binary", parsed.Headers["DataType"]);
        Assert.Equal(data.Length.ToString(), parsed.Headers["ContentLength"]);
        Assert.Equal(data, parsed.GetBinaryPayload());
    }

    [Fact]
    public void Parse_CanHandle_CRLF_and_LF_Endings()
    {
        // Build a raw frame using LF only to ensure parser tolerates it
        var builder = new StringBuilder();
        builder.AppendLine("PING");
        builder.AppendLine("DataType: text");
        builder.AppendLine();
        builder.Append("pong");

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        var parsed = Frame.Parse(new ReadOnlyMemory<byte>(bytes));

        Assert.Equal("PING", parsed.Command);
        Assert.Equal("text", parsed.Headers["DataType"]);
        Assert.Equal("pong", parsed.GetTextPayload());
    }
}
