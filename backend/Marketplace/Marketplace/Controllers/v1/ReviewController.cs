using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.Review;
using Marketplace.Core.Exceptions;
using Marketplace.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    public class ReviewController : CustomControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] ReviewQueryParameters parameters)
        {
            var result = await _reviewService.GetReviews(parameters);
            return result.IsSuccess
                ? OkResponse(new { comments = result.Data.comments, totalCount = result.Data.totalCount })
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("me/byProduct")]
        [ValidateToken]
        public async Task<IActionResult> GetMyReviewByProduct([FromQuery] long productId)
        {
            var result = await _reviewService.GetReviewByUserAndProduct(UserId.Value, productId);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("me")]
        [ValidateToken]
        public async Task<IActionResult> GetMyReview()
        {
            var result = await _reviewService.GetReviewByUserId(UserId.Value);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("byUserAndProduct")]
        public async Task<IActionResult> GetReviewByUserAndProductIds([FromQuery] long userId, [FromQuery] long productId)
        {
            var result = await _reviewService.GetReviewByUserAndProduct(userId, productId);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpGet("me/can-leave")]
        [ValidateToken]
        public async Task<IActionResult> CanLeaveReview([FromQuery] long productId)
        {
            var result = await _reviewService.CanLeaveReview(UserId.Value, productId);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpPost]
        [ValidateModel]
        [ValidateToken]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequest request)
        {
            request.UserId = UserId.Value;
            var result = await _reviewService.AddReview(request);
            return result.IsSuccess
                ? OkResponse(result.Data)
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpDelete("{reviewId:long}")]
        public async Task<IActionResult> DeleteReviewById(long reviewId)
        {
            var result = await _reviewService.DeleteReviewById(reviewId);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }

        [HttpDelete("me")]
        [ValidateToken]
        public async Task<IActionResult> DeleteMyReview([FromQuery] long productId)
        {
            var result = await _reviewService.DeleteReview(productId, UserId.Value);
            return result.IsSuccess
                ? NoContentResponse()
                : ErrorResponse(result.ErrorMessage, result.StatusCode);
        }
    }
}
