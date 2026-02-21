using Authentication.Models.Dto;

namespace Authentication.Service.IService
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        //Task<bool> AssignRole(string email, string roleName);
    }
}
