using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.Cart;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CartController : CustomControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var result = await _cartService.GetCart();
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("{cartId:long}")]
        public async Task<IActionResult> GetCartById(long cartId)
        {
            var result = await _cartService.GetCartByCartId(cartId);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("byUser/{id:long}")]
        public async Task<IActionResult> GetCartByUserId(long id)
        {
            var result = await _cartService.GetCartByUserId(id);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("self")]
        public async Task<IActionResult> GetCartSelf()
        {
            if (!UserId.HasValue)
                return ErrorResponse("Token not valid", 401);

            var result = await _cartService.GetCartByUserId(UserId.Value);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("items/{id:long}")]
        public async Task<IActionResult> GetCartItem(long id)
        {
            var result = await _cartService.GetCartItemById(id);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "user")]
        [HttpPost("items")]
        [ValidateModel]
        [ValidateToken]
        public async Task<IActionResult> AddCartItem([FromBody] CartItemCreateRequest request)
        {
            var result = await _cartService.AddCartItem(request, UserId.Value);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "user")]
        [HttpPatch("items/{itemId:long}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateCartItem(long itemId, [FromBody] CartItemUpdateRequest request)
        {
            request.CartItemId = itemId;
            var result = await _cartService.UpdateCartItem(request);
            return result.IsSuccess ? NoContentResponse() : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "user")]
        [HttpDelete("items/{itemId:long}")]
        public async Task<IActionResult> RemoveCartItem(long itemId)
        {
            var result = await _cartService.DeleteCartItem(itemId);
            return result.IsSuccess ? NoContentResponse() : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

    }
}
