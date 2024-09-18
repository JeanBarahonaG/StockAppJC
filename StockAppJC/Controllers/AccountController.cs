using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockAppJC.Models;
using StockAppJC.Services;
using StockAppJC.Token;

namespace StockAppJC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("SingIn")]
        public async Task<IActionResult> SignIn([FromBody] Usuario user)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest("Data Invalid");
            }
            var userSearch = await _userManager.FindByEmailAsync(user.Email);
            if (userSearch == null)
            {
                return BadRequest("User not found");
            }
            var result = await _signInManager.PasswordSignInAsync(userSearch, user.PasswordHash, false, false);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid Email or Password");
            }

            var token = await _tokenService.GenerateTokenAsync(userSearch);

            return Ok(new 
            {
                code = 200,
                description = "Successful Sign In",
                token = token
            });
        }

        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOut() 
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) 
            {
                return BadRequest("Token not found");
            }

            TokenRevocationManager.RevokeToken(token);

            return Ok(new
            {
                code = 200,
                description = "Successful Sign Out"
            });
        }

        [HttpGet("ValidateToken")]
        public async Task<IActionResult> ValidateToken(string token) 
        {
            bool response = await _tokenService.TokenValidation(token);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = response });
        }
    }
}
