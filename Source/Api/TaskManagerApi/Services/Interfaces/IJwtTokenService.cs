using TaskManagerApi.Models;

namespace TaskManagerApi.Services
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        bool IsAccessTokenExpired(string token);
        string? GetUserNameFromToken(string token);
        
    }
}