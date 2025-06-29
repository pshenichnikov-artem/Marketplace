using Marketplace.Core.DTOs.User;

namespace Marketplace.Core.DTOs.Cart
{
    public class CartResponse
    {
        public long Id { get; set; }
        public List<CartItemResponse> Items { get; set; } = new();
        public UserSelfResponse User { get; set; }
    }
}
