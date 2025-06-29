using Marketplace.Core.Response;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(
                        x => x.Key,
                        x => string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))
                    );

                var response = ApiResponse.CreateFailure("Неверный данные", 404, errors);

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}
