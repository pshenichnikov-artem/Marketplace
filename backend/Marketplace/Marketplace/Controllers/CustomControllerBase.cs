using Marketplace.Core.Response;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Marketplace.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CustomControllerBase : ControllerBase
    {
        protected long? UserId =>
        long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;


        protected IActionResult OkResponse(object data, string message = "Operation success")
        {
            return StatusCode(200, ApiResponse.CreateSuccess(data, message));
        }

        protected IActionResult ErrorResponse(string message, int statusCode)
        {
            return StatusCode(400, ApiResponse.CreateFailure(message, 400));
        }

        protected IActionResult NoContentResponse()
        {
            return StatusCode(200, ApiResponse.CreateSuccess(null));
        }
    }
}
