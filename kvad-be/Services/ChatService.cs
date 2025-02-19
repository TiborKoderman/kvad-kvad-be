using kvad_be.Migrations;
using Microsoft.AspNetCore.Mvc;

public class ChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }
    public Task<String> CreateChatRoom(string chatRoomName, User user)
    {
        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = chatRoomName,
            Users = new List<User>(),
            Messages = new List<ChatMessage>(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        chatRoom.Users.Add(user);

        _context.ChatRooms.Add(chatRoom);
        _context.SaveChanges();
        return Task.FromResult(chatRoom.Name);
    }

    public Task<List<ChatRoomDTO>> GetChatRooms(User user)
    {
        var chatRooms = _context.ChatRooms.Where(cr => cr.Users.Contains(user)).ToList();
        var chatRoomDTOs = chatRooms.Select(cr => new ChatRoomDTO(cr.Id, cr.Name, cr.Users, cr.CreatedAt, cr.UpdatedAt)).ToList();
        return Task.FromResult(chatRoomDTOs);
    }

    public Task<ChatRoom> GetChatRoom(Guid chatRoomId)
    {
        var chatRoom = _context.ChatRooms.FirstOrDefault(cr => cr.Id == chatRoomId);
        if (chatRoom == null)
        {
            throw new Exception("Chat room not found");
        }
        return Task.FromResult(chatRoom);
    }

    //get chats in a chat room
    public Task<List<ChatMessage>> GetChatMessages(Guid chatRoomId)
    {
        var chatMessages = _context.ChatMessages.Where(cm => cm.ChatRoomId == chatRoomId).ToList();
        return Task.FromResult(chatMessages);
    }

    public Task AddChatMessage(Guid chatRoomId, User user, string content)
    {
        var chatRoom = _context.ChatRooms.FirstOrDefault(cr => cr.Id == chatRoomId);
        if (chatRoom == null)
        {
            throw new Exception("Chat room not found");
        }

        var chatMessage = new ChatMessage
        {
            ChatRoomId = chatRoomId,
            User = user,
            Content = content,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // chatRoom.UpdatedAt = DateTime.Now;
        _context.ChatMessages.Add(chatMessage);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task AddUserToChatRoom(Guid chatRoomId, User user)
    {
        var chatRoom = _context.ChatRooms.FirstOrDefault(cr => cr.Id == chatRoomId);
        if (chatRoom == null)
        {
            throw new Exception("Chat room not found");
        }
        chatRoom.Users.Add(user);
        _context.SaveChanges();
        return Task.CompletedTask;
    }
    

}