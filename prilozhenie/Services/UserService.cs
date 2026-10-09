using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prilozhenie.DatabaseContext;
using prilozhenie.Interfaces;
using prilozhenie.Models;
using prilozhenie.Requests;

namespace prilozhenie.Services
{
    public class UserService : IUserService
    {
        private readonly ContextDb _context;

        public UserService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    id_User = u.Id_User,
                    Name = u.Name,
                    Description = u.Description,
                    Role = u.Role != null ? u.Role.RoleName : string.Empty,
                    Email = u.Login != null ? u.Login.Email : string.Empty
                })
                .ToListAsync();

            return new OkObjectResult(new
            {
                data = new { users = users },
                status = true
            });
        }

        public async Task<IActionResult> CreateNewUserAndLoginAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Email и пароль обязательны"
                });
            }

            var existing = await _context.Logins
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (existing != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Пользователь с таким Email уже существует"
                });
            }

            var user = new User
            {
                Name = request.Name ?? string.Empty,
                Description = request.Description ?? string.Empty,
                RoleId = 2
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new Login
            {
                Email = request.Email,
                Password = request.Password,
                User_id = user.Id_User
            };

            await _context.Logins.AddAsync(login);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                userId = user.Id_User
            });
        }

        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var login = await _context.Logins
                .Include(l => l.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(l => l.Email == request.Email);

            if (login == null || login.Password != request.Password)
            {
                return new UnauthorizedObjectResult(new
                {
                    status = false,
                    message = "Неверный Email или пароль"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Вход выполнен успешно",
                userId = login.User.Id_User,
                userName = login.User.Name,
                description = login.User.Description,
                role = login.User.Role?.RoleName
            });
        }

        public async Task<IActionResult> UpdateUserAsync(UserUpdateRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            if (!string.IsNullOrEmpty(request.Name))
                user.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Description))
                user.Description = request.Description;

            if (request.RoleId.HasValue)
                user.RoleId = request.RoleId.Value;

            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Пользователь обновлён"
            });
        }

        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Login)
                .FirstOrDefaultAsync(u => u.Id_User == id);

            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            if (user.Login != null)
                _context.Logins.Remove(user.Login);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Пользователь удалён"
            });
        }
    }
}