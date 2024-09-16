namespace TaskManagerApi.Models.Dtos
{
    public class UserDto
    {
        public UserDto() { }
        
        public UserDto(User user)
        {
            UserName = user.UserName;
            Email = user.Email;
        }
        
        public UserDto(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        public string? UserName { get; set; }
        public string? Email { get; set; }        
    }
}