namespace Marketplace.Core.DTOs.Order
{
    public class AdminOrderResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
