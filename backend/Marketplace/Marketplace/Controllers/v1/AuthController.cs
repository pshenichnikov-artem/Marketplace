using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.User;
using Marketplace.Core.Exceptions;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AuthController : CustomControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("sign-in")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userData)
        {
            var result = await _userService.AuthenticateAsync(userData);
            return result.IsSuccess
                ? OkResponse(result.Data, "Login successful")
                : ErrorResponse("Invalid password", 401);
        }

        [AllowAnonymous]
        [HttpPost("sign-up")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] UserCreateRequest userCreateRequest)
        {
            var result = await _userService.RegisterAsync(userCreateRequest);
            return result.IsSuccess
                ? OkResponse(result.Data, "Registration successful")
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize]
        [HttpGet("status")]
        public IActionResult IsAuthorized()
        {
            return Ok();
        }
    }
}
