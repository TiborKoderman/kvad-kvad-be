using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

[Route("ws")]
[AllowAnonymous]
[ApiController]
public class WebSocketController : ControllerBase
{

  private readonly TopicHub _topicHub;
  private readonly AuthService _auth;

  public WebSocketController(TopicHub topicHub, AuthService authService)
  {
    _topicHub = topicHub;
    _auth = authService;
  }

  [HttpGet("")]
  public async Task Get()
  {
    var user = await _auth.GetUser(User);
    if (HttpContext.WebSockets.IsWebSocketRequest)
    {
      await _topicHub.ConnectClientAsync(HttpContext, user);
    }
    else
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
  }

}
