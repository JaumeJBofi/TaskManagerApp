using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Dtos.Authentication
{
    public class SignInDto
    {
        public SignInDto() { }        
        public string? UserName { get; set; }        
        public string? Password { get; set; }        
        public string? Email { get; set; }
    }
}
