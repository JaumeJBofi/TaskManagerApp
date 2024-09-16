namespace TaskManagerApi.Services.Interfaces
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password,string hash);
        int GetPasswordSecurityRating(string password);
    }
}