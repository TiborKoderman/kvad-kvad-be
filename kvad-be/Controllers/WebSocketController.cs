using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kvad_be.Services.WebSocket;

[Route("ws")]
[ApiController]
[AllowAnonymous]
internal class WebSocketController(TopicHub topicHub, AuthService authService) : ControllerBase
{
  [HttpGet("")]
  public async Task Get()
  {

    if (HttpContext.WebSockets.IsWebSocketRequest)
    {
      var user = await authService.GetUser(User);
      await topicHub.ConnectClientAsync(HttpContext, user);
    }
    else
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
  }

}
