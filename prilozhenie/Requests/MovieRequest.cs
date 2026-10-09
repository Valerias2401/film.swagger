using System.ComponentModel.DataAnnotations;

namespace prilozhenie.Requests
{
    public class MovieRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? GenreId { get; set; }
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public string? PosterUrl { get; set; }
    }
}