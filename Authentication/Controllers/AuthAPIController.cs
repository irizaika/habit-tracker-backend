using Authentication.Models.Dto;
using Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;

        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var registerResponse = await _authService.Register(model);

            return Ok(registerResponse);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);

            return Ok(loginResponse);

        }

        //[HttpPost("assignRole")]
        //public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        //{
        //    var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
        //    if (!assignRoleSuccessful)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = "Error encountered";
        //        return BadRequest(_response);
        //    }
        //    return Ok(_response);

        //}
    }
}
