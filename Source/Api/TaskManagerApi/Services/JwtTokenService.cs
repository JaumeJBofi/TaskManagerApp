using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TaskManagerApi.Models;
using TaskManagerApi.Utilities;

namespace TaskManagerApi.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IConfig config)
        {
            _jwtSettings = config.JwtSettings;
        }

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = RSA.Create();
            key.ImportRSAPrivateKey(_jwtSettings.JwtKey, out _);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),                    
                }),

                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(key), SecurityAlgorithms.RsaSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {            
             var refreshToken = Guid.NewGuid().ToString();
             return refreshToken;
        }

        public bool VerifyToken(string token, User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = RSA.Create();
            key.ImportRSAPrivateKey(_jwtSettings.JwtKey, out _);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,                    
                    ValidAudience = _jwtSettings.ValidAudience,
                    ValidIssuer = _jwtSettings.ValidIssuer
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var userName = principal.FindFirstValue(ClaimTypes.Name);

                if (userName == user.UserName)
                {
                    return true;
                }
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