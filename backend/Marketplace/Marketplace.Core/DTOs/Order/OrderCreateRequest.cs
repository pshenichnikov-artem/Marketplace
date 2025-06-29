namespace Marketplace.Core.DTOs.Order
{
    public class OrderCreateRequest
    {
        public string ShippingAddress { get; set; } = null!;
        public List<OrderItemRequest> Items { get; set; } = new();
    }
}
