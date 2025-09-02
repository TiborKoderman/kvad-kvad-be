using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }


    [HttpPost("newChatRoom")]
    public async Task<IActionResult> NewChatRoom(NewChatRoomDTO chatroom)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        if (chatroom.Name == null || string.IsNullOrWhiteSpace(chatroom.Name))
        {
            return BadRequest("Invalid chat room name");
        }
        var chatRoom = await _chatService.CreateChatRoom(chatroom.Name, user);
        if (chatRoom == null)
        {
            return BadRequest("Chat room already exists");
        }

        return Ok(chatRoom);
    }

    [HttpGet("chatRooms")]
    public async Task<IActionResult> GetChatRooms()
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        var chatRooms = await _chatService.GetChatRooms(user);
        return Ok(chatRooms);
    }

    [HttpGet("chatRoom/{id}")]
    public async Task<IActionResult> GetChatRoom(Guid id)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        var chatRoom = await _chatService.GetChatRoom(id);
        return Ok(chatRoom);
    }

    [HttpGet("chatMessages/{id}")]
    public async Task<IActionResult> GetChatMessages(Guid id)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        var chatMessages = await _chatService.GetChatMessages(id);
        return Ok(chatMessages);
    }

    [HttpPost("sendMessage")]
    public async Task<IActionResult> AddChatMessage(SendChatMessageDTO chatMessage)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        if (chatMessage.Content == null || string.IsNullOrWhiteSpace(chatMessage.Content))
        {
            return BadRequest("Invalid chat message content");
        }
        await _chatService.AddChatMessage(chatMessage.ChatRoomId, user, chatMessage.Content);
        return Ok();
    }

    [HttpDelete("deleteChatRoom/{id}")]
    public async Task<IActionResult> DeleteChatRoom(Guid id)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized();
        }
        await _chatService.DeleteChatRoom(id, user);
        return Ok();
    }
}
