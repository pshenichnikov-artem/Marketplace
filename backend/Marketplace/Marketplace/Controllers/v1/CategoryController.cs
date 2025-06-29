using Marketplace.Core.DTOs.Category;
using Marketplace.Core.Response;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers.v1
{
    public class CategoriesController : CustomControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null, int? parentId = null)
        {
            var result = await _categoryService.GetCategories(search, parentId);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId:long}")]
        public async Task<IActionResult> GetById(long categoryId)
        {
            var result = await _categoryService.GetCategoryById(categoryId);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
        {
            var result = await _categoryService.AddCategory(request);
            return result.IsSuccess
                ? Created("", ApiResponse.CreateSuccess(result.Data))
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{categoryId:long}")]
        public async Task<IActionResult> Update(long categoryId, [FromBody] CategoryCreateRequest request)
        {
            var result = await _categoryService.UpdateCategory(categoryId, request);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{categoryId:long}")]
        public async Task<IActionResult> DeleteCategory(long categoryId)
        {
            var result = await _categoryService.DeleteCategory(categoryId);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }
    }
}
