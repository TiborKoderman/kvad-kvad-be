using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("ws")]
[ApiController]
public class WebSocketController : ControllerBase
{
  private readonly TopicHub _topicHub;

  public WebSocketController(TopicHub topicHub)
  {
    _topicHub = topicHub;
  }

  [HttpGet("")]
  public async Task Get()
  {

    if (HttpContext.WebSockets.IsWebSocketRequest)
    {
      await _topicHub.ConnectClientAsync(HttpContext);
    }
    else
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
  }

}
