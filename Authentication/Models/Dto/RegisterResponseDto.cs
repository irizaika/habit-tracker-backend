namespace Authentication.Models.Dto
{
    public class RegisterResponseDto
    {
        public List<string> Messages { get; set; } = [];
        public bool IsSuccessful { get; set; } = true;
    }
}
   