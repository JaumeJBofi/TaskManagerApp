using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Models.Dtos.Authentication
{
    public class LogoutDto
    {
        public LogoutDto() { }
        [Required]
        public required string UserName { get; set; }        
    }
}