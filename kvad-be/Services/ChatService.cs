using Microsoft.AspNetCore.Mvc;

public class ChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }
    public Task<ChatRoomDTO> CreateChatRoom(string chatRoomName, User user)
    {
        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = chatRoomName,
            Users = new List<User>(),
            Messages = new List<ChatMessage>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        chatRoom.Users.Add(user);
        ChatRoomDTO chatRoomDTO = new ChatRoomDTO(chatRoom.Id, chatRoom.Name, chatRoom.Users , chatRoom.CreatedAt, chatRoom.UpdatedAt);


        _context.ChatRooms.Add(chatRoom);
        _context.SaveChanges();
        return Task.FromResult(chatRoomDTO);
    }

    public Task<List<ChatRoomDTO>> GetChatRooms(User user)
    {
        var chatRooms = _context.ChatRooms.Where(cr => cr.Users.Contains(user)).OrderByDescending(cr => cr.UpdatedAt).ToList();
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
    public Task<List<ChatMessageDTO>> GetChatMessages(Guid chatRoomId)
    {
        var chatMessages = _context.ChatMessages
            .Where(cm => cm.ChatRoomId == chatRoomId)
            .OrderBy(cm => cm.CreatedAt)
            .ToList();
        var chatMessageDTOs = chatMessages
            .Select(cm => new ChatMessageDTO(cm.Id, cm.User.Id, cm.User.Username, cm.Content, cm.CreatedAt, cm.UpdatedAt))
            .ToList();
        return Task.FromResult(chatMessageDTOs);
    }

    public async Task AddChatMessage(Guid chatRoomId, User user, string content)
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        chatRoom.UpdatedAt = DateTime.UtcNow;
        await _context.ChatMessages.AddAsync(chatMessage);

        await _context.SaveChangesAsync();
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

    public Task DeleteChatRoom(Guid chatRoomId, User user)
    {
        var chatRoom = _context.ChatRooms.FirstOrDefault(cr => cr.Id == chatRoomId);
        if (chatRoom == null)
        {
            throw new Exception("Chat room not found");
        }
        if (!chatRoom.Users.Contains(user))
        {
            throw new Exception("User is not a member of the chat room");
        }
        _context.ChatRooms.Remove(chatRoom);
        _context.SaveChanges();
        return Task.CompletedTask;
    }
    

}