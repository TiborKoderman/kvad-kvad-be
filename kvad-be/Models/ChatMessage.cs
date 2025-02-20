using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Id), nameof(ChatRoomId))]
public class ChatMessage
{
    public int Id { get; set; } // Changed to long
    [ForeignKey("ChatRoom")]
    public required Guid ChatRoomId { get; set; }
    public required User User { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = null;
}