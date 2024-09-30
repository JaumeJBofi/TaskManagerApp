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
        private readonly IConfig _config;

        public JwtTokenService(IConfig config)
        {
            _config = config;
            _jwtSettings = config.JwtSettings;
            _signingKey = new SymmetricSecurityKey(_jwtSettings.JwtKey);            
        }

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimsConstants.Role, RoleConstants.User)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Audience = _config.JwtSettings.ValidAudience,
                Issuer = _config.JwtSettings.ValidIssuer,
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.CreateEncodedJwt(tokenDescriptor);
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
                    ValidateIssuer = true,
                    ValidateAudience = true,                    
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
            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        public string? GetUserIdFromToken(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;            
        }

    }
}