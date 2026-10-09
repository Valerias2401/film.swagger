using System.ComponentModel.DataAnnotations;

namespace prilozhenie.Requests
{
    public class LoginRequest
    {
        [MinLength(1, ErrorMessage = "Email не может быть пустым")]
        public string? Email { get; set; }
        [MinLength(1, ErrorMessage = "Пароль не может быть пустым")]
        public string? Password { get; set; }
    }
}