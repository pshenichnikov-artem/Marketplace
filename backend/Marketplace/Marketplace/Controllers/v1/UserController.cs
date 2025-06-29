using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.User;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserController : CustomControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 20, string? search = null, string role = null)
        {
            var result = await _userService.GetUsers(page, pageSize, search, role);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "user, seller")]
        [HttpGet("self")]
        [ValidateToken]
        public async Task<IActionResult> GetSelf()
        {
            var result = await _userService.GetUserById(UserId.Value);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var result = await _userService.GetUserById(id);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser([FromRoute] long id, [FromBody] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUser(id, request);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpPut()]
        [ValidateToken]
        [ValidateModel]
        public async Task<IActionResult> UpdateUserSelf([FromBody] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUser(UserId.Value, request);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var result = await _userService.DeleteUser(id);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }
    }
}
