namespace Authentication.Models.Dto
{
    public class UserResponseDto
    {
        public UserDto User { get; set; } = new UserDto();
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn {get; set;}
    }
}
   