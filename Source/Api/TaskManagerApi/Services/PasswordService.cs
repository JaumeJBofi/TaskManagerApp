
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {            
            return BCrypt.Net.BCrypt.HashPassword(password);             
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public int GetPasswordSecurityRating(string password)
        {            
            var score = Zxcvbn.Core.EvaluatePassword(password);            
            return score.Score;
        }
    }
}