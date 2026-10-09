using System.ComponentModel.DataAnnotations;

namespace prilozhenie.Requests
{
    public class UserUpdateRequest
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? RoleId { get; set; }
    }
}