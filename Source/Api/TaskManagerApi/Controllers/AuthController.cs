using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos;
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
        private readonly IAntiforgery _antiforgery;


        public AuthController(IConfig config, IUserRepository userRepository, IJwtTokenService jwtTokenService, IPasswordService passwordService, IAntiforgery antiforgery)
        {
            _config = config;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _passwordService = passwordService;
            _antiforgery = antiforgery;
        }


        [HttpPost]
        [Authorize]            
        public ActionResult IsUserLogged()
        {
            return Ok();
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

            var token = _jwtTokenService.GenerateAccessToken(user);      

            await _userRepository.UpdateUserRefreshToken(login.UserName);

            await SetRefreshTokenCookie(user);
            
            var csrfToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;            

            return Ok(new
            {
                AccessToken = token,      
                XsrfToken = csrfToken,
                UserDto = new UserDto
                {
                    UserName = user.UserName,
                    Email = user.Email
                }
            });
        }

        [HttpPost]        
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loggedUser = await GetUser();
          

            Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if(refreshToken == null) return BadRequest("Refresh token not found");

            if (await _userRepository.RevokeRefreshToken(loggedUser.UserName, refreshToken))
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
            var tokenInfo = await _userRepository.UpdateUserRefreshToken(user.UserName); 

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,                
                Expires = tokenInfo.expireDate,
                Secure = true,
                SameSite = SameSiteMode.None                
            };
            
            Response.Cookies.Append("refreshToken", tokenInfo.token, cookieOptions);            
        }

        private async Task<User> GetUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User id not found in claims");
            var user = await _userRepository.GetByIdAsync(userId);
            return user ?? throw new Exception("User not found");
        }
    }
}