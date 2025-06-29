using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Core.DTOs.Product
{
    public class ProductCreateRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public short StockQuantity { get; set; }
        public long CategoryId { get; set; }
        public long? SellerId { get; set; }
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
        public List<string> OldPhotos { get; set; } = new List<string>();
        public int PrimaryPhotoIndex { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Проверяем, что хотя бы один элемент есть в Photos или OldPhotos
            if (!(Photos.Any() || OldPhotos.Any()))
            {
                yield return new ValidationResult("At least one photo must be provided in either 'Photos' or 'OldPhotos'.", new[] { nameof(Photos), nameof(OldPhotos) });
            }

            // Дополнительная валидация для PrimaryPhotoIndex
            if (PrimaryPhotoIndex >= Photos.Count && PrimaryPhotoIndex >= OldPhotos.Count)
            {
                yield return new ValidationResult("PrimaryPhotoIndex is invalid.", new[] { nameof(PrimaryPhotoIndex) });
            }
        }
    }
}
