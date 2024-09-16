using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Dtos.Authentication
{
    public class SignInDto
    {
        public SignInDto() { }
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required string Email { get; set; }
    }
}
