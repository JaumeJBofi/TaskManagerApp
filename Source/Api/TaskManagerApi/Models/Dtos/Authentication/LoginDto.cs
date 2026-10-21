using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Dtos.Authentication
{
    public class LoginDto
    {
        public LoginDto() {}        
        public  string? UserName { get; set; }
        public  string? Password { get; set; }
    }
}