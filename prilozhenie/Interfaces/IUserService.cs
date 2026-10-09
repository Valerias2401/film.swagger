using Microsoft.AspNetCore.Mvc;
using prilozhenie.Requests;

namespace prilozhenie.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> GetAllUsersAsync();
        Task<IActionResult> CreateNewUserAndLoginAsync(RegisterRequest request);
        Task<IActionResult> LoginAsync(LoginRequest request);
        Task<IActionResult> UpdateUserAsync(UserUpdateRequest request);
        Task<IActionResult> DeleteUserAsync(int id);
    }
}