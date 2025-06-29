namespace Marketplace.Core.DTOs.Review
{
    public class ReviewQueryParameters
    {
        public long? ProductId { get; set; }
        public long? UserId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; }
        public string? OrderDirection { get; set; }
    }
}
