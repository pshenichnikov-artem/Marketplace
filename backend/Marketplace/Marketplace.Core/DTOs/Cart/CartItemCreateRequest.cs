namespace Marketplace.Core.DTOs.Cart
{
    public class CartItemCreateRequest
    {
        public long ProductId { get; set; }
        public short Quantity { get; set; }
    }
}
