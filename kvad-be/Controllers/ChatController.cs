using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace kvad_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(ChatService chatService, AuthService authService) : ControllerBase
{
    [HttpPost("newChatRoom")]
    public async Task<IActionResult> NewChatRoom(NewChatRoomDTO chatroom)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        if (chatroom.Name == null || string.IsNullOrWhiteSpace(chatroom.Name))
        {
            return BadRequest("Invalid chat room name");
        }
        var chatRoom = await chatService.CreateChatRoom(chatroom.Name, user);
        if (chatRoom == null)
        {
            return BadRequest("Chat room already exists");
        }

        return Ok(chatRoom);
    }

    [HttpGet("chatRooms")]
    public async Task<IActionResult> GetChatRooms()
    {

        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var chatRooms = await chatService.GetChatRooms(user);
        return Ok(chatRooms);
    }

    [HttpGet("chatRoom/{id}")]
    public async Task<IActionResult> GetChatRoom(Guid id)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var chatRoom = await chatService.GetChatRoom(id);
        return Ok(chatRoom);
    }

    [HttpGet("chatMessages/{id}")]
    public async Task<IActionResult> GetChatMessages(Guid id)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        var chatMessages = await chatService.GetChatMessages(id);
        return Ok(chatMessages);
    }

    [HttpPost("sendMessage")]
    public async Task<IActionResult> AddChatMessage(SendChatMessageDTO chatMessage)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        if (chatMessage.Content == null || string.IsNullOrWhiteSpace(chatMessage.Content))
        {
            return BadRequest("Invalid chat message content");
        }
        await chatService.AddChatMessage(chatMessage.ChatRoomId, user, chatMessage.Content);
        return Ok();
    }

    [HttpDelete("deleteChatRoom/{id}")]
    public async Task<IActionResult> DeleteChatRoom(Guid id)
    {
        var user = await authService.GetUser(User);
        if (user == null)
        {
            return Unauthorized();
        }
        await chatService.DeleteChatRoom(id, user);
        return Ok();
    }
}
