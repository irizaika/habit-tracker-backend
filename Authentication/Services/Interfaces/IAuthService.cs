using Authentication.Models.Dto;

namespace Authentication.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        //Task<bool> AssignRole(string email, string roleName);
    }
}
