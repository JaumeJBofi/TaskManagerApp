using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagerApi.Models;
using TaskManagerApi.Utilities;

namespace TaskManagerApi.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SymmetricSecurityKey _signingKey;

        public JwtTokenService(IConfig config)
        {
            _jwtSettings = config.JwtSettings;
            _signingKey = new SymmetricSecurityKey(_jwtSettings.JwtKey);
        }

        public (string token, ClaimsPrincipal claimsPrincipal) GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (token: tokenHandler.CreateEncodedJwt(tokenDescriptor), claimsPrincipal);
        }

        public string GenerateRefreshToken()
        {            
             var refreshToken = Guid.NewGuid().ToString();
             return refreshToken;
        }

        public bool VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
 
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,                    
                    ValidAudience = _jwtSettings.ValidAudience,
                    ValidIssuer = _jwtSettings.ValidIssuer
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var userName = principal.FindFirstValue(ClaimTypes.Name);
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public bool IsAccessTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.ValidTo < DateTime.Now;
        }

        public string? GetUserNameFromToken(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            return userName;
        }

    }
}