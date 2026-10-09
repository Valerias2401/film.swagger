using System.ComponentModel.DataAnnotations;

namespace prilozhenie.Models
{
    public class Role
    {
        [Key]
        public int Id_Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public virtual ICollection<User>? Users { get; set; }
    }
}