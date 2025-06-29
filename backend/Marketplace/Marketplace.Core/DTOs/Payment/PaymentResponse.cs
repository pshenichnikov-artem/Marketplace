namespace Marketplace.Core.DTOs.Payment
{
    public class PaymentResponse
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
