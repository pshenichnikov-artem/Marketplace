namespace Marketplace.Core.DTOs.Payment
{
    public class PaymentCreateRequest
    {
        public long OrderId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
