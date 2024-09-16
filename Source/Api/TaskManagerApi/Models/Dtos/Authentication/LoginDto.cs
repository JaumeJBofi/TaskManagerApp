using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Dtos.Authentication
{
    public class LoginDto
    {
        public LoginDto() {}        
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}