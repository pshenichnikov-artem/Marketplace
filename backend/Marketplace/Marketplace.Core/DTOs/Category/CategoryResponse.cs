namespace Marketplace.Core.DTOs.Category
{
    public class CategoryResponse
    {
        public long Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public long? ParentCategoryId { get; set; }
        public List<CategoryResponse> SubCategories { get; set; } = new List<CategoryResponse>();
    }
}
