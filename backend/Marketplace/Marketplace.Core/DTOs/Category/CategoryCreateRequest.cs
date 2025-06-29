namespace Marketplace.Core.DTOs.Category
{
    public class CategoryCreateRequest
    {
        public string? CategoryName { get; set; }

        public long? ParentCategoryId { get; set; } 
    }
}
