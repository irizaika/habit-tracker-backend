using Authentication.Data;
using Authentication.Models;
using Authentication.Models.Dto;
using Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager/*, RoleManager<IdentityRole> roleManager*/)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            //_roleManager = roleManager;
        }

        //public async Task<bool> AssignRole(string email, string roleName)
        //{
        //    var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        //    if (user != null)
        //    {
        //        if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
        //        {
        //            //create role if it does not exist
        //            _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
        //        }
        //        await _userManager.AddToRoleAsync(user, roleName);
        //        return true;
        //    }
        //    return false;

        //}

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName != null && u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            if (user == null )
            {
                return new LoginResponseDto() { Result = null, IsSuccess = false, Message = "User does not exist." };
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (isValid == false)
            {
                return new LoginResponseDto() { Result = null, IsSuccess = false, Message = "Wrong password" };
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDTO = new()
            {
                Email = user.Email ?? "",
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber ?? ""
            };

            LoginResponseDto loginResponseDto = new()
            {
                Result = new UserResponseDto()
                {
                    User = userDTO,
                    Token = token,
                    ExpiresIn = 7 * 24 * 60 * 60 // 7 days in seconds, to do
                },
                IsSuccess = true,
                Message = "Login successful"
            };

            return loginResponseDto;
        }

        public async Task<RegisterResponseDto> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                //check if user exists
                var userNameExists = _userManager.Users.Any(u => u.UserName == user.UserName);

                if (userNameExists)
                {
                    return new RegisterResponseDto()
                    {
                        Messages = new List<string>() { $"User name {user.UserName} already exists" },
                        IsSuccessful = false
                    };
                }

                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email ?? "",
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber ?? ""
                    };

                    return new RegisterResponseDto()
                    {
                        IsSuccessful = true,
                        Messages = new List<string>() { "User is registered" }
                    };

                }
                else
                {
                    return new RegisterResponseDto()
                    {
                        Messages = [.. result.Errors.Select(e => e.Description)],
                        IsSuccessful = false
                    };
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
            }

            return new RegisterResponseDto()
            {
                Messages = new List<string>() { "Error Encountered" },
                IsSuccessful = false
            };
        }
    }
}
