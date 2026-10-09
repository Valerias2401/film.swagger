using System.ComponentModel.DataAnnotations;

namespace prilozhenie.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        public string GenreName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public virtual ICollection<Movie>? Movies { get; set; }
    }
}