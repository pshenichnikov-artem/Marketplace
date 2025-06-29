using Marketplace.Core.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Marketplace.API.Attributes
{
    public class ValidateTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult(ApiResponse.CreateFailure("Token is missing or invalid", 401));
                return;
            }


            var token = authorizationHeader.Replace("Bearer ", "");

            try
            {

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    context.Result = new UnauthorizedObjectResult(ApiResponse.CreateFailure("Invalid token format", 401));
                    return;
                }

                // Извлекаем UserId из токена
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out _))
                {
                    context.Result = new UnauthorizedObjectResult(ApiResponse.CreateFailure("Invalid token: UserId is missing or invalid", 401));
                    return;
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedObjectResult(ApiResponse.CreateFailure("Error while validating token", 401));
            }
        }
    }
}
