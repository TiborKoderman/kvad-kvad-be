using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ChatService
{
    private readonly AppDbContext _context;

    // private readonly List<WebSocket> connections = [];

    private static ConcurrentDictionary<Guid, HashSet<WebSocket>> connections = new ConcurrentDictionary<Guid, HashSet<WebSocket>>();

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
            Users = [],
            Messages = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        chatRoom.Users.Add(user);
        ChatRoomDTO chatRoomDTO = new(chatRoom.Id, chatRoom.Name, chatRoom.Users , chatRoom.CreatedAt, chatRoom.UpdatedAt);


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
        var chatRoom = _context.ChatRooms.Include(cr => cr.Users).FirstOrDefault(cr => cr.Id == chatRoomId);
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

        chatRoom.Users.ForEach(async u =>
        {
            if (connections.TryGetValue(u.Id, out HashSet<WebSocket>? sockets) && sockets != null)
            {
                foreach (var socket in sockets)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        var message = Encoding.UTF8.GetBytes(chatMessage.Content);
                        var messageSegment = new ArraySegment<byte>(message);
                        await socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        });

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


    public Task AddSocket(WebSocket socket, Guid userId)
    {
        if (connections.TryGetValue(userId, out HashSet<WebSocket>? sockets) && sockets != null)
        {
            sockets.Add(socket);
        }
        else
        {
            connections.TryAdd(userId, new HashSet<WebSocket> { socket });
        }
        return Task.CompletedTask;
    }

    public Task RemoveSocket(WebSocket socket, Guid userId)
    {
        if (connections.TryGetValue(userId, out HashSet<WebSocket>? sockets) && sockets != null)
        {
            sockets.Remove(socket);
        }
        return Task.CompletedTask;
    }

    public async Task Receive(WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                return;
            }
            else if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer).Trim('\0');
                var messageBytes = Encoding.UTF8.GetBytes(message);
                var messageSegment = new ArraySegment<byte>(messageBytes);
                foreach (var (userId, sockets) in connections)
                {
                    foreach (var userSocket in sockets)
                    {
                        if (userSocket.State == WebSocketState.Open)
                        {
                            await userSocket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
        }
    }
}