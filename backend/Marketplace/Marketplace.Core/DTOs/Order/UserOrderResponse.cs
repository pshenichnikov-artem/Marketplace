using Marketplace.Core.DTOs.Payment;

namespace Marketplace.Core.DTOs.Order
{
    public class UserOrderResponse
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public PaymentResponse? Payment { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
