namespace Marketplace.Core.DTOs.Product
{
    public class ProductImageRequest
    {
        public string Url { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
