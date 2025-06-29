using Marketplace.Core.DTOs.Product;

namespace Marketplace.Core.DTOs.Cart
{
    public class CartItemResponse
    {
        public long Id { get; set; }
        public ProductResponse Product { get; set; }
        public short Quantity { get; set; }
    }
}
