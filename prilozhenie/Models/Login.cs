using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prilozhenie.Models
{
    public class Login
    {
        [Key]
        public int id_Login { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public int User_id { get; set; }
        public virtual User? User { get; set; }
    }
}