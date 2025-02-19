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

    [HttpGet("chatMessages/{id}")]
    public async Task ChatMessages(Guid id)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var chatRoom = await _chatService.GetChatRoom(id);
            if (chatRoom == null)
            {
                return;
            }
            var user = HttpContext.Items["User"] as User;
            if (user == null)
            {
                return;
            }
            if (!chatRoom.Users.Contains(user))
            {
                return;
            }
            var chatMessages = await _chatService.GetChatMessages(chatRoom.Id);
            foreach (var message in chatMessages)
            {
                var bytes = Encoding.UTF8.GetBytes(message.ToString());
                var ArraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                if (ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(ArraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (ws.State == WebSocketState.CloseReceived || ws.State == WebSocketState.Aborted)
                {
                    break;
                }
            }
            while (true)
            {
                // var message = await ReceiveMessage(ws);
                // if (message == null)
                // {
                //     break;
                // }
                // await _chatService.AddChatMessage(chatRoom, user, message);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

}
