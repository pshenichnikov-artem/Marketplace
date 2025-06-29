using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.Product;
using Marketplace.Core.Entities;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProductsController : CustomControllerBase
    {
        private readonly ProductService _productService;


        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters queryParameters)
        {

            long? sellerIdParsed = null;
            if (queryParameters.SellerId == "my")
            {
                if (UserId == null) return ErrorResponse("Token not valid", 401);
                sellerIdParsed = UserId.Value;
            }
            else if (long.TryParse(queryParameters.SellerId, out var parsedId))
            {
                sellerIdParsed = parsedId;
            }

            var result = await _productService.GetProductsAsync(queryParameters, sellerIdParsed);

            return result.IsSuccess
                ? OkResponse(new { Products = result.Data.products, TotalCount = result.Data.totalCount })
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProductById(long id, [FromServices] ApplicationDbContext db)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (result.IsSuccess && UserId.HasValue)
            {
                var userProductHistory = await db.UserProductHistories
                    .FirstOrDefaultAsync(u => u.ProductId == id && u.UserId == UserId.Value);
                if (userProductHistory != null)
                {
                    userProductHistory.CreatedAt = DateTime.UtcNow;
                    db.UserProductHistories.Update(userProductHistory);
                }
                else
                {
                    await db.UserProductHistories.AddAsync(new UserProductHistory
                    {
                        ProductId = id,
                        UserId = UserId.Value,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await db.SaveChangesAsync();

            }

            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "seller,admin")]
        [HttpPost]
        [ValidateModel]
        [ValidateToken]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateRequest request)
        {
            if (request.Photos.Count == 0)
                return ErrorResponse("Необходимо загрузить хотя бы одно фото.", 400);

            request.SellerId = UserId;

            var result = await _productService.AddProductAsync(request);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "seller, admin")]
        [HttpPut("{id:long}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProduct(long id, [FromForm] ProductCreateRequest request)
        {
            var result = await _productService.UpdateProductAsync(id, request);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "seller, admin")]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }
    }
}
