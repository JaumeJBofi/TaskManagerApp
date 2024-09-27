using System.Security.Claims;
using TaskManagerApi.Models;

namespace TaskManagerApi.Services
{
    public interface IJwtTokenService
    {
        (string token,ClaimsPrincipal claimsPrincipal) GenerateAccessToken(User user);
        string GenerateRefreshToken();
        bool IsAccessTokenExpired(string token);
        string? GetUserNameFromToken(string token);

        bool VerifyToken(string token);


    }
}