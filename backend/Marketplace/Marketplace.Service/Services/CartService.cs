using AutoMapper;
using Marketplace.API;
using Marketplace.Core.DTOs.Cart;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Core.Response;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Service.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CartService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<CartResponse>>> GetCart()
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(c => c.Product)
                        .ThenInclude(c => c.ProductImages)
                .Include(c => c.User)
                .ToListAsync();

            var mapped = _mapper.Map<IEnumerable<CartResponse>>(cart);
            return ServiceResult<IEnumerable<CartResponse>>.Success(mapped);
        }

        public async Task<ServiceResult<CartResponse>> GetCartByUserId(long id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(c => c.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Userid == id);

            if (cart == null)
                return ServiceResult<CartResponse>.Failure("Cart not found", 404);

            return ServiceResult<CartResponse>.Success(_mapper.Map<CartResponse>(cart));
        }

        public async Task<ServiceResult<CartResponse>> GetCartByCartId(long id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return ServiceResult<CartResponse>.Failure("Cart not found", 404);

            return ServiceResult<CartResponse>.Success(_mapper.Map<CartResponse>(cart));
        }

        public async Task<ServiceResult<CartResponse>> GetCartById(long id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(c => c.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return ServiceResult<CartResponse>.Failure("Cart not found", 404);

            return ServiceResult<CartResponse>.Success(_mapper.Map<CartResponse>(cart));
        }

        public async Task<ServiceResult<CartItemResponse>> GetCartItemById(long id)
        {
            var cartItem = await _context.CartItems
                .Include(ct => ct.Product)
                .FirstOrDefaultAsync(ct => ct.Id == id);

            if (cartItem == null)
                return ServiceResult<CartItemResponse>.Failure("Cart item not found", 404);

            return ServiceResult<CartItemResponse>.Success(_mapper.Map<CartItemResponse>(cartItem));
        }

        public async Task<ServiceResult<CartItemResponse>> AddCartItem(CartItemCreateRequest cartItemRequest, long userId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItemRequest.ProductId);
            if (product == null)
                return ServiceResult<CartItemResponse>.Failure("Product not found", 404);

            if (product.StockQuantity < cartItemRequest.Quantity)
                return ServiceResult<CartItemResponse>.Failure("Not enough stock", 400);

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Userid == userId);
            if (cart == null)
                return ServiceResult<CartItemResponse>.Failure("Cart not found", 404);

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == cartItemRequest.ProductId && c.CartId == cart.Id);

            if (existingItem != null)
                return ServiceResult<CartItemResponse>.Failure("Cart item already exists", 409);

            var newCartItem = new CartItem
            {
                Quantity = cartItemRequest.Quantity,
                ProductId = cartItemRequest.ProductId,
                CartId = cart.Id
            };

            await _context.CartItems.AddAsync(newCartItem);
            product.StockQuantity -= cartItemRequest.Quantity;
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CartItemResponse>(newCartItem);
            return ServiceResult<CartItemResponse>.Success(result, 200);
        }

        public async Task<ServiceResult<bool>> UpdateCartItem(CartItemUpdateRequest updateRequest)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == updateRequest.CartItemId);
            if (cartItem == null)
                return ServiceResult<bool>.Failure("Cart item not found", 404);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
            if (product == null)
                return ServiceResult<bool>.Failure("Product not found", 404);

            if (product.StockQuantity < updateRequest.Quantity)
                return ServiceResult<bool>.Failure("Not enough stock", 400);

            cartItem.Quantity = updateRequest.Quantity;
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }

        public async Task<ServiceResult<bool>> DeleteCartItem(long id)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ct => ct.Id == id);
            if (cartItem == null)
                return ServiceResult<bool>.Failure("Cart item not found", 404);

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }
    }
}
