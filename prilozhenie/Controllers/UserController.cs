using Microsoft.AspNetCore.Mvc;
using prilozhenie.Interfaces;
using prilozhenie.Requests;

namespace prilozhenie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

        [HttpPost]
        [Route("createNewUserAndLogin")]
        public async Task<IActionResult> CreateNewUserAndLogin([FromBody] RegisterRequest request)
        {
            return await _userService.CreateNewUserAndLoginAsync(request);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return await _userService.LoginAsync(request);
        }

        [HttpPut]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest request)
        {
            return await _userService.UpdateUserAsync(request);
        }

        [HttpDelete]
        [Route("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return await _userService.DeleteUserAsync(id);
        }
    }
}