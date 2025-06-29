using AutoMapper;
using Marketplace.API;
using Marketplace.Core.DTOs.Cart;
using Marketplace.Core.DTOs.Order;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Core.Response;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Marketplace.Service.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public async Task<ServiceResult<(IEnumerable<UserOrderResponse> orders, int totalCount)>> GetOrderAsync(
        int page, int pageSize, int? userId, long? sellerId, int? productId,
        string? status, string? orderBy, DateTime? dateTo, DateTime? dateFrom)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Include(o => o.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            if (sellerId.HasValue)
                query = query.Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId.Value));

            if (productId.HasValue)
                query = query.Where(o => o.OrderItems.Any(oi => oi.Productid == productId.Value));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (dateFrom.HasValue)
                query = query.Where(o => o.OrderDate >= dateFrom.Value);

            if (dateTo.HasValue)
                query = query.Where(o => o.OrderDate <= dateTo.Value);

            int totalCount = await query.CountAsync();

            query = orderBy switch
            {
                "date_asc" => query.OrderBy(o => o.OrderDate),
                "status" => query.OrderBy(o => o.Status),
                _ => query.OrderByDescending(o => o.OrderDate)
            };

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<IEnumerable<UserOrderResponse>>(orders);
            return ServiceResult<(IEnumerable<UserOrderResponse>, int)>.Success((mapped, totalCount));
        }

        public async Task<ServiceResult<UserOrderResponse>> GetOrderById(long id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return ServiceResult<UserOrderResponse>.Failure("Order not found", 404);

            var mapped = _mapper.Map<UserOrderResponse>(order);
            return ServiceResult<UserOrderResponse>.Success(mapped);
        }

        public async Task<ServiceResult<bool>> UpdateOrder(long orderId, OrderUpdateRequest request)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return ServiceResult<bool>.Failure("Order not found", 404);

            order.Status = request.Status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }

        public async Task<ServiceResult<bool>> AddOrder(OrderCreateRequest request, long userId)
        {
            var order = new Order
            {
                UserId = userId,
                Status = "pending",
                OrderDate = DateTime.UtcNow,
                OrderItems = request.Items.Select(item => new OrderItem
                {
                    Productid = item.ProductId,
                    Quantity = item.Quantity,
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }

        public async Task<ServiceResult<bool>> DeleteOrder(long orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return ServiceResult<bool>.Failure("Order not found", 404);

            order.Status = "canceled";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }

        //public async Task<OrderItemResponse> GetOrderItemById(long id)
        //{
        //    var cartItem = await _context.OrderItems
        //        .FirstOrDefaultAsync(ct => ct.Id == id);

        //    return _mapper.Map<OrderItemResponse>(cartItem);
        //}

        //public async Task AddOrder(OrderCreateRequest cartItemRequest, long userId)
        //{
        //    Order order = new Order()
        //    {
        //        ShippingAddress = cartItemRequest.ShippingAddress,
        //        UserId = userId,
        //        Status = "Ждет обработки",
        //    };

        //    await _context.Orders.AddAsync(order);
        //    await _context.SaveChangesAsync();

        //    foreach (var item in cartItemRequest.Items)
        //    {
        //        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
        //        if (product == null)
        //        {
        //            throw new Exception();
        //        }

        //        OrderItem newItem = new OrderItem()
        //        {
        //            Productid = item.ProductId,
        //            OrderId = order.Id,
        //            Quantity = item.Quantity,
        //            Price = product.Price,
        //        };
        //        await _context.AddAsync(newItem);
        //    }
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateStatus(long id, string status)
        //{
        //    var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        //    if (order == null)
        //    {
        //        throw new NotFoundException();
        //    }

        //    order.Status = status;
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteOrder(long id)
        //{
        //    var orderItem = await _context.Orders.FirstOrDefaultAsync(ct => ct.Id == id);
        //    if (orderItem == null)
        //    {
        //        throw new NotFoundException("Cart item not found");
        //    }

        //    orderItem.Status = "Отменен";
        //    await _context.SaveChangesAsync();
        //}
    }
}
