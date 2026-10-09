using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prilozhenie.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [ForeignKey("Genre")]
        public int GenreId { get; set; }
        public virtual Genre? Genre { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        [Required]
        [Range(0, 10)]
        public double Rating { get; set; }

        public string? PosterUrl { get; set; }
    }
}