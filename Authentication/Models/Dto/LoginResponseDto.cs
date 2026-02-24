namespace Authentication.Models.Dto
{
    public class LoginResponseDto
    {
        public UserResponseDto? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
