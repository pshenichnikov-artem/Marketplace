using AutoMapper;
using Marketplace.API;
using Marketplace.Core.DTOs.Product;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Core.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Marketplace.Service.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<(IEnumerable<ProductResponse> products, int totalCount)>> GetProductsAsync(
            ProductQueryParameters queryParameters,
            long? sellerId)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(queryParameters.Search))
                    query = query.Where(p =>
                        p.Description.ToLower().Contains(queryParameters.Search.ToLower()) ||
                        p.Name.ToLower().Contains(queryParameters.Search.ToLower()));

                if (queryParameters.CategoryId != null)
                    query = query.Where(p => p.CategoryId == queryParameters.CategoryId);

                if (sellerId != null)
                    query = query.Where(p => p.SellerId == sellerId);

                // Считаем общее количество для пагинации
                var totalCount = await query.CountAsync();

                // Определяем сортировку
                Expression<Func<Product, object>> orderByExs = queryParameters.OrderBy switch
                {
                    "price" => t => t.Price,
                    "priceAsc" => t => t.Price,
                    "name" => t => t.Name,
                    "rating" => p => p.Reviews.Average(r => r.Rating),
                    "new" => t => t.CreatedAt,
                    _ => p =>
                        0.3 * p.Reviews.Count() +
                        p.Reviews.Sum(r => r.Rating <= 3 ? 0.5 * r.Rating : 1.5 * r.Rating)
                };

                bool descending = queryParameters.OrderBy is not ("priceAsc" or "name");

                query = query
                    .Include(p => p.ProductImages)
                    .Include(p => p.Reviews)
                    .Include(p => p.Seller);

                query = descending
                    ? query.OrderByDescending(orderByExs)
                    : query.OrderBy(orderByExs);

                query = query
                    .Skip(queryParameters.Page * 50)
                    .Take(50);

                var products = await query.ToListAsync();
                var mapped = _mapper.Map<IEnumerable<ProductResponse>>(products);

                return ServiceResult<(IEnumerable<ProductResponse>, int)>.Success((mapped, totalCount));
            }
            catch (Exception ex)
            {
                return ServiceResult<(IEnumerable<ProductResponse>, int)>.Failure("Error retrieving products: " + ex.Message, 500);
            }
        }

        public async Task<ServiceResult<ProductResponse>> GetProductByIdAsync(long id)
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync();

            if (product == null)
                return ServiceResult<ProductResponse>.Failure("Product not found", 404);

            var mapped = _mapper.Map<ProductResponse>(product);
            return ServiceResult<ProductResponse>.Success(mapped);
        }

        public async Task<ServiceResult<long>> AddProductAsync(ProductCreateRequest productRequest)
        {
            var product = _mapper.Map<Product>(productRequest);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var productImg = new List<ProductImage>();
            foreach (var file in productRequest.Photos)
            {
                var fileName = file.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                productImg.Add(new ProductImage()
                {
                    ImageUrl = $"https://localhost:7037/images/{fileName}",
                    IsPrimary = false,
                    Productid = product.Id,
                });
            }

            productImg[productRequest.PrimaryPhotoIndex].IsPrimary = true;

            await _context.ProductImages.AddRangeAsync(productImg);
            await _context.SaveChangesAsync();

            return ServiceResult<long>.Success(product.Id, 201);
        }

        public async Task<ServiceResult<bool>> UpdateProductAsync(long id, ProductCreateRequest productRequest)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return ServiceResult<bool>.Failure("Product not found", 404);

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == productRequest.CategoryId);
            if (category == null)
                return ServiceResult<bool>.Failure("Category not found", 404);

            product.Name = productRequest.Name;
            product.Description = productRequest.Description;
            product.Price = productRequest.Price;
            product.StockQuantity = productRequest.StockQuantity;
            product.CategoryId = productRequest.CategoryId;

            var oldImages = await _context.ProductImages.Where(p => p.Productid == product.Id).ToListAsync();
            foreach (var image in oldImages)
            {
                string imageUrl = image.ImageUrl;
                int startIndex = imageUrl.IndexOf("images/") + "images/".Length;
                string fileName = imageUrl.Substring(startIndex);

                if (!productRequest.OldPhotos.Contains(imageUrl))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    _context.ProductImages.Remove(image);
                }
            }
            await _context.SaveChangesAsync();

            var newImages = new List<ProductImage>();

            foreach (var file in productRequest.Photos)
            {
                var fileName = file.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                newImages.Add(new ProductImage()
                {
                    ImageUrl = $"https://localhost:7037/images/{fileName}",
                    IsPrimary = false,
                    Productid = product.Id,
                });
            }

            oldImages.ForEach(i => i.IsPrimary = false);
            newImages.ForEach(i => i.IsPrimary = false);

            int idx = productRequest.PrimaryPhotoIndex;
            if (idx < oldImages.Count)
                oldImages[idx].IsPrimary = true;
            else if (idx - oldImages.Count < newImages.Count)
                newImages[idx - oldImages.Count].IsPrimary = true;
            else
                if (oldImages.Count > 0)
                oldImages[0].IsPrimary = true;
            else if (newImages.Count > 0)
                newImages[0].IsPrimary = true;

            await _context.ProductImages.AddRangeAsync(newImages);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }

        public async Task<ServiceResult<bool>> DeleteProductAsync(long id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(u => u.Id == id);
            if (product == null)
                return ServiceResult<bool>.Failure("Product not found", 404);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, 200);
        }
    }
}
