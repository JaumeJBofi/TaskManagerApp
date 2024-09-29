using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos.Authentication;

namespace TaskManagerApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User> CreateAsync(SignInDto newUserDto);
        Task<int> DeleteAsync(string id);
        Task DeleteByUserNameAsync(string userName);
        Task<(string token,DateTime expireDate)> UpdateUserRefreshToken(string userName);
        Task<bool> RevokeRefreshToken(string userName, string refreshToken);
        Task<bool> ValidateRefreshToken(string userName, string refreshToken);
    }
}