namespace Marketplace.Core.DTOs.Order
{
    public class OrderItemRequest
    {
        public long ProductId { get; set; }
        public short Quantity { get; set; }
    }
}
