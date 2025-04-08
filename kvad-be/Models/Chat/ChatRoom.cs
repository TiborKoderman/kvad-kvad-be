public class ChatRoom
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required  List<User> Users { get; set; } = [];
    public required  List<ChatMessage> Messages { get; set; } = [];
    public required  DateTime CreatedAt { get; set; }
    public required  DateTime UpdatedAt { get; set; }
}