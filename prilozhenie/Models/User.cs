using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prilozhenie.Models
{
    public class User
    {
        [Key]
        public int Id_User { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int RoleId { get; set; } = 2;
        public virtual Role? Role { get; set; }

        public virtual Login? Login { get; set; }
    }
}