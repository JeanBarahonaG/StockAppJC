using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockAppJC.General;
using StockAppJC.Models.ViewModels;
using StockAppJC.Services;

namespace StockAppJC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUser")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<ActionResult<UserViewModel>> GetUser(string id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            Task<IEnumerable<UserViewModel>> users = _userService.GetUsers();
            return await users;
        }

        [HttpPost("RegisterUser")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<ActionResult<answer>> RegisterUser(RegisterUserViewModel user)
        {
            var result = await _userService.RegisterUser(user);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPut("UpdateUser")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<ActionResult<answer>> UpdateUser(string id, UpdateUserViewModel user)
        {
            var result = await _userService.UpdateUser(id, user);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPatch("UpdatePassword")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<ActionResult<answer>> UpdatePassword(string id, UpdatePasswordUserViewModel password)
        {
            var result = await _userService.UpdatePassword(id, password);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<ActionResult<answer>> DeleteUser(string id)
        {
            var result = await _userService.DeleteUser(HttpContext, id);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
