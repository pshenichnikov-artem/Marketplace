namespace Marketplace.Core.DTOs.Order
{
    public class OrderUpdateRequest
    {
        public string ShippingAddress { get; set; } = null!;
        public string Status { get; set; } = null!;
        public List<OrderItemRequest> Items { get; set; } = new();
    }
}
