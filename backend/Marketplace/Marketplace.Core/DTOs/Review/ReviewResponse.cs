namespace Marketplace.Core.DTOs.Review
{
    public class ReviewResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long ProductId { get; set; }
        public short Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
