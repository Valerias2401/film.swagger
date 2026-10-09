using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prilozhenie.Models
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }
        public int? MovieId { get; set; }
        public int FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(MovieId))]
        public virtual Movie? Movie { get; set; }
        [ForeignKey(nameof(FromUserId))]
        public virtual User? FromUser { get; set; }
        [ForeignKey(nameof(ToUserId))]
        public virtual User? ToUser { get; set; }
    }
}