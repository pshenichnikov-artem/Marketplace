using AutoMapper;
using Marketplace.Core.DTOs.Cart;
using Marketplace.Core.DTOs.Category;
using Marketplace.Core.DTOs.Order;
using Marketplace.Core.DTOs.Payment;
using Marketplace.Core.DTOs.Product;
using Marketplace.Core.DTOs.Review;
using Marketplace.Core.DTOs.User;
using Marketplace.Core.Entities;

namespace Marketplace.Service.Mappers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            // Category
            CreateMap<Category, CategoryResponse>();
            CreateMap<CategoryCreateRequest, Category>();

            // User
            CreateMap<User, AdminUserResponse>();
            CreateMap<User, UserSelfResponse>();
            CreateMap<UserUpdateRequest, User>();
            CreateMap<UserCreateRequest, User>();
            CreateMap<User, SellerResponse>();

            // Product
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src =>
                        src.Reviews.Average(rev => (double?)rev.Rating) != null
                            ? Math.Round((decimal)src.Reviews.Average(rev => (double?)rev.Rating).Value, 2)
                            : (decimal?)null))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));
            CreateMap<ProductImage, ProductImageResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.ImageUrl));
            CreateMap<ProductCreateRequest, Product>();
            CreateMap<ProductImageRequest, ProductImage>();

            // Cart
            CreateMap<Cart, CartResponse>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));
            CreateMap<CartItem, CartItemResponse>();
            CreateMap<CartItemUpdateRequest, CartItem>();
            CreateMap<CartItemCreateRequest, CartItem>();

            // Order
            CreateMap<Order, AdminOrderResponse>();
            CreateMap<Order, UserOrderResponse>()
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payments.FirstOrDefault()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<OrderItem, OrderItemResponse>();
            CreateMap<OrderCreateRequest, Order>();
            CreateMap<OrderItemRequest, OrderItem>();

            // Review
            CreateMap<Review, ReviewResponse>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));
            CreateMap<ReviewCreateRequest, Review>();

            // Payment
            CreateMap<Payment, PaymentResponse>();
            CreateMap<PaymentCreateRequest, Payment>();
        }
    }
}
