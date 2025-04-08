
[Microsoft.EntityFrameworkCore.PrimaryKey(nameof(UserId), nameof(ChatRoomId))]
public class UserChatRoom
{
    public required User User { get; set; }
    public required Guid UserId { get; set; }
    public required ChatRoom ChatRoom { get; set; }
    public required Guid ChatRoomId { get; set; }
    public byte[]? PrivateKey { get; set; } = null;
}