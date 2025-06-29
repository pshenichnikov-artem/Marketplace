using AutoMapper;
using Marketplace.API;
using Marketplace.Core.DTOs.Category;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Core.Response;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Service.Services;

public class CategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(IMapper mapper, ApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<CategoryResponse>>> GetCategories(string? categoryName, int? parentId = null)
    {
        var query = _context.Categories
            .Include(c => c.InverseParentCategory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(c => c.CategoryName.StartsWith(categoryName));
        }

        if (parentId.HasValue)
        {
            query = query.Where(c => c.ParentCategoryId == parentId.Value);
        }

        var categories = await query.ToListAsync();
        var result = BuildCategoryTree(categories, null).OrderByDescending(c => c.CategoryName);
        return ServiceResult<IEnumerable<CategoryResponse>>.Success(result);
    }

    public async Task<ServiceResult<CategoryResponse>> GetCategoryById(long id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return ServiceResult<CategoryResponse>.Failure("Category not found", 404);

        var categoryResponse = _mapper.Map<CategoryResponse>(category);
        var allCategories = await _context.Categories.ToListAsync();
        categoryResponse.SubCategories = BuildCategoryTree(allCategories, category.Id);

        return ServiceResult<CategoryResponse>.Success(categoryResponse);
    }

    public async Task<ServiceResult<long>> AddCategory(CategoryCreateRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.CategoryName))
            return ServiceResult<long>.Failure("Invalid category data", 400);

        if (request.ParentCategoryId != null)
        {
            var parent = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId);
            if (parent == null)
                return ServiceResult<long>.Failure("Parent category not found", 404);
        }

        var newCategory = _mapper.Map<Category>(request);
        await _context.Categories.AddAsync(newCategory);
        await _context.SaveChangesAsync();

        return ServiceResult<long>.Success(newCategory.Id, 201);
    }

    public async Task<ServiceResult<bool>> UpdateCategory(long id, CategoryCreateRequest request)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return ServiceResult<bool>.Failure("Category not found", 404);

        if (request.ParentCategoryId != null)
        {
            var parent = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId);
            if (parent == null)
                return ServiceResult<bool>.Failure("Parent category not found", 404);
        }

        category.CategoryName = request.CategoryName ?? category.CategoryName;
        category.ParentCategoryId = request.ParentCategoryId;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Success(true, 200);
    }

    public async Task<ServiceResult<bool>> DeleteCategory(long id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return ServiceResult<bool>.Failure("Category not found", 404);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Success(true, 200);
    }

    private List<CategoryResponse> BuildCategoryTree(List<Category> categories, long? parentId)
    {
        return categories
            .Where(c => c.ParentCategoryId == parentId)
            .Select(c =>
            {
                var response = _mapper.Map<CategoryResponse>(c);
                response.SubCategories = BuildCategoryTree(categories, c.Id);
                return response;
            })
            .ToList();
    }
}