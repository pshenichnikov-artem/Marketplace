namespace Marketplace.Core.DTOs.Review
{
    public class ReviewCreateRequest
    {
        public long UserId { get; set; }
        public long ProductId { get; set; }
        public short Rating { get; set; }
        public string Comment { get; set; } = null!;
    }
}
