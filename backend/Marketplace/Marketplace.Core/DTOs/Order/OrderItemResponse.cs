namespace Marketplace.Core.DTOs.Order
{
    public class OrderItemResponse
    {
        public long ProductId { get; set; }
        public long OrderId { get; set; }
        public decimal Price { get; set; }
        public short Quantity { get; set; }
    }
}
