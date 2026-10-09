using System.ComponentModel.DataAnnotations;

namespace prilozhenie.Requests
{
    public class RegisterRequest
    {
        [MinLength(1, ErrorMessage = "Имя не может быть пустым")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [MinLength(1, ErrorMessage = "Email не может быть пустым")]
        public string? Email { get; set; }
        [MinLength(6, ErrorMessage = "Пароль должен быть минимум 6 символов")]
        public string? Password { get; set; }
    }
}