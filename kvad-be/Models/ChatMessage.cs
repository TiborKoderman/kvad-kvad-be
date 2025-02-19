using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

public class ChatMessage
{
    [Key]
    public required Guid Id { get; set; } // Changed to long
    [ForeignKey("ChatRoom")]
    public required Guid ChatRoomId { get; set; }
    public required User User { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = null;
}