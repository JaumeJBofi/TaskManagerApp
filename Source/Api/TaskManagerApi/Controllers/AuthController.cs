using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos.Authentication;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Services;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Utilities;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {        
        private readonly IConfig _config;        
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordService _passwordService;

        public AuthController(IConfig config, IUserRepository userRepository, IJwtTokenService jwtTokenService, IPasswordService passwordService)
        {
            _config = config;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _passwordService = passwordService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInDto signIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _userRepository.GetByUserNameAsync(signIn.UserName);

            if(user != null) return Unauthorized(new { message = "Username already exist" });

            if (_passwordService.GetPasswordSecurityRating(signIn.Password) < 3)
            {
                return Unauthorized(new { message = "Password not secure enough" });
            }

            var newUser = await _userRepository.CreateAsync(signIn);
            
            var accessToken = _jwtTokenService.GenerateAccessToken(newUser);
            await SetRefreshTokenCookie(newUser);

            await _userRepository.UpdateUserRefreshToken(newUser.UserName);

            return Ok(new
            {
                AccessToken = accessToken,                
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _userRepository.GetByUserNameAsync(login.UserName);

            if (user == null || !_passwordService.VerifyPassword(login.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            await SetRefreshTokenCookie(user);

            await _userRepository.UpdateUserRefreshToken(login.UserName);

            return Ok(new
            {
                AccessToken = accessToken,                
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logout)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetByUserNameAsync(logout.UserName);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username" });
            }

            Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if(refreshToken == null) return BadRequest("Refresh token not found");

            if (await _userRepository.RevokeRefreshToken(logout.UserName, refreshToken))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Could not log out user");
            };            
        }
        
        private async Task SetRefreshTokenCookie(User user)
        {
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var dateTime = await _userRepository.UpdateUserRefreshToken(user.UserName); 

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                Expires = dateTime.AddHours(1),
                SameSite = SameSiteMode.Strict 
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);            
        }
    }
}