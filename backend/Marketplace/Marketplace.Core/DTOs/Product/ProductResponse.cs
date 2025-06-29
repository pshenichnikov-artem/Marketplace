using Marketplace.Core.DTOs.User;

namespace Marketplace.Core.DTOs.Product
{
    public class ProductResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public short StockQuantity { get; set; }
        public long CategoryId { get; set; }
        public SellerResponse Seller { get; set; } = new();
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<ProductImageResponse> Images { get; set; } = new();
    }
}
