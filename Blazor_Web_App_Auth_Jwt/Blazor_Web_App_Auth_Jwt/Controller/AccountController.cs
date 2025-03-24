using Blazor_Web_App_Auth.Application;
using Blazor_Web_App_Auth.Application.Interfaces;
using Blazor_Web_App_Auth.Domain.Models;
using Blazor_Web_App_Auth.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blazor_Web_App_Auth_Jwt.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly ITokenHandler _tokenHandler;

        public AuthController(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel request)
        {
            
            var user = await _userRepository.GetUserByEmail(request.Email!);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            bool passwordCheck = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordCheck)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token =  _jwtHelper.GenerateToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();
            return Ok(new LoginResponse { Token = token ,RefreshToken=refreshToken});
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var UsertoRegister = user;
            await _userRepository.AddUserAsync(UsertoRegister);
            return Ok();

        }
    }
}
