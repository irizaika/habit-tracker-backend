namespace Authentication.Models.Dto
{
    public class UserResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        public int ExpiresIn {get; set;}
    }
}
   