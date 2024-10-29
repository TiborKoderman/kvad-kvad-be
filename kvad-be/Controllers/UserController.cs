using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kvad_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO user)
        {
            await _userService.registerUser(user);
            return Ok();
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.getUser(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.getUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.getUsers();
            return Ok(users);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            await _userService.updateUser(user);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] User user)
        {
            await _userService.deleteUser(user);
            return Ok();
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateUser([FromBody] RegisterUserDTO user)
        {
            var authenticatedUser = await _userService.authenticateUser(user.Username, user.Password);
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }
            return Ok(authenticatedUser);
        }
    }
}