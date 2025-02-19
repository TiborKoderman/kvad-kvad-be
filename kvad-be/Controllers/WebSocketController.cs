using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

[ApiController]
[Route("ws")]
[AllowAnonymous]
public class WebSocketController : ControllerBase
{

    private readonly ChatService _chatService;

    [HttpGet("time")]
    public async Task Time()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            while (true)
            {
                var message = "The current time is " + DateTime.Now.ToString("h:mm:ss tt");
                var bytes = Encoding.UTF8.GetBytes(message);
                var ArraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                if (ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(ArraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (ws.State == WebSocketState.CloseReceived || ws.State == WebSocketState.Aborted)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    // [HttpGet("chatRoom/{id}")]
    // public async Task ChatRoom(Guid id)
    // {
        
        
    // }

}
