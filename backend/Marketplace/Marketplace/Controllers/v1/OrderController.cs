using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.Order;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OrdersController : CustomControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, int pageSize = 20, int? userId = null, string? sellerId = null, int? productId = null, string? status = null, string? orderBy = null, DateTime? dateTo = null, DateTime? dateFrom = null)
        {
            long? sellerIdParsed = null;
            if (sellerId == "my")
            {
                if (UserId == null) return ErrorResponse("Token not valid", 401);
                sellerIdParsed = UserId.Value;
            }
            else if (sellerId != null && long.TryParse(sellerId, out var parsedId))
            {
                sellerIdParsed = parsedId;
            }

            var result = await _orderService.GetOrderAsync(page, pageSize, userId, sellerIdParsed, productId, status, orderBy, dateTo, dateFrom);
            return result.IsSuccess
                ? OkResponse(new { Orders = result.Data.orders, TotalItem = result.Data.totalCount })
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [AllowAnonymous]
        [HttpGet("{orderId:long}")]
        public async Task<IActionResult> GetById([FromRoute] long orderId)
        {
            var result = await _orderService.GetOrderById(orderId);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }
        [Authorize(Roles = "seller, admin")]
        [HttpPatch("{orderId:long}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateOrder([FromRoute] long orderId,
            [FromBody] OrderUpdateRequest request)
        {

            var result = await _orderService.UpdateOrder(orderId, request);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateModel]
        [ValidateToken]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
        {
            var result = await _orderService.AddOrder(request, UserId.Value);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "user")]
        [HttpDelete("{orderId:long}")]
        public async Task<IActionResult> CancelOrder([FromRoute] long orderId)
        {
            var result = await _orderService.DeleteOrder(orderId);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }
    }
}
