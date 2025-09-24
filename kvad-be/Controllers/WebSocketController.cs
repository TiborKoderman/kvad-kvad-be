using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("ws")]
[ApiController]
[AllowAnonymous]
public class WebSocketController : ControllerBase
{
  private readonly TopicHub _topicHub;
  private readonly AuthService _authService;

  public WebSocketController(TopicHub topicHub, AuthService authService)
  {
    _topicHub = topicHub;
    _authService = authService;
  }

  [HttpGet("")]
  public async Task Get()
  {

    if (HttpContext.WebSockets.IsWebSocketRequest)
    {
    var user = await _authService.GetUser(User);
      await _topicHub.ConnectClientAsync(HttpContext, user);
    }
    else
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
  }

}
