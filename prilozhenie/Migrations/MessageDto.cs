namespace prilozhenie.Models
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int? MovieId { get; set; }
        public int FromUserId { get; set; }
        public string FromUserName { get; set; } = string.Empty;
        public int? ToUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
    }
}