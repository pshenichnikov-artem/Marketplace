using AutoMapper;
using Marketplace.API;
using Marketplace.Core.DTOs.Review;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Core.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Service.Services
{
    public class ReviewService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResult<(IEnumerable<ReviewResponse> comments, int totalCount)>> GetReviews(ReviewQueryParameters parameters)
        {
            var query = _context.Reviews.Include(r => r.User).AsQueryable();

            if (parameters.ProductId != null)
                query = query.Where(r => r.ProductId == parameters.ProductId);

            if (parameters.UserId != null)
                query = query.Where(r => r.UserId == parameters.UserId);

            if (!string.IsNullOrEmpty(parameters.Search))
                query = query.Where(r => r.Comment.ToLower().Contains(parameters.Search.ToLower()));

            if (!string.IsNullOrEmpty(parameters.OrderBy))
            {
                bool desc = parameters.OrderDirection?.ToLower() == "desc";
                query = parameters.OrderBy switch
                {
                    "userId" => desc ? query.OrderByDescending(r => r.UserId) : query.OrderBy(r => r.UserId),
                    "productId" => desc ? query.OrderByDescending(r => r.ProductId) : query.OrderBy(r => r.ProductId),
                    "createdAt" => desc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
                    "comment" => desc ? query.OrderByDescending(r => r.Comment) : query.OrderBy(r => r.Comment),
                    "rating" => desc ? query.OrderByDescending(r => r.Rating) : query.OrderBy(r => r.Rating),
                    _ => query.OrderByDescending(r => r.CreatedAt)
                };
            }

            int totalCount = await query.CountAsync();
            var reviews = await query
                .Skip(parameters.Page * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
            return ServiceResult<(IEnumerable<ReviewResponse>, int)>.Success((result, totalCount));
        }

        public async Task<ServiceResult<ReviewResponse>> GetReviewByUserAndProduct(long userId, long productId)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);

            return review == null
                ? ServiceResult<ReviewResponse>.Failure("Review not found", 404)
                : ServiceResult<ReviewResponse>.Success(_mapper.Map<ReviewResponse>(review));
        }

        public async Task<ServiceResult<ReviewResponse>> GetReviewByUserId(long userId)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);

            return review == null
                ? ServiceResult<ReviewResponse>.Failure("Review not found", 404)
                : ServiceResult<ReviewResponse>.Success(_mapper.Map<ReviewResponse>(review));
        }

        public async Task<ServiceResult<bool>> CanLeaveReview(long userId, long productId)
        {
            var hasPurchased = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .AnyAsync(o => o.OrderItems.Any(oi => oi.Productid == productId));

            if (!hasPurchased)
                return ServiceResult<bool>.Failure("User has not purchased this product", 403);

            var alreadyLeft = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId);
            return ServiceResult<bool>.Success(!alreadyLeft);
        }

        public async Task<ServiceResult<ReviewResponse>> AddReview(ReviewCreateRequest reviewRequest)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == reviewRequest.ProductId);
            if (!productExists)
                return ServiceResult<ReviewResponse>.Failure("Product not found", 404);

            var userExists = await _context.Users.AnyAsync(u => u.Id == reviewRequest.UserId);
            if (!userExists)
                return ServiceResult<ReviewResponse>.Failure("User not found", 404);

            var review = _mapper.Map<Review>(reviewRequest);
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return ServiceResult<ReviewResponse>.Success(_mapper.Map<ReviewResponse>(review));
        }

        public async Task<ServiceResult<bool>> DeleteReview(long productId, long userId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
            if (review == null)
                return ServiceResult<bool>.Failure("Review not found", 404);

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Success(true, 200);
        }

        public async Task<ServiceResult<bool>> DeleteReviewById(long id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null)
                return ServiceResult<bool>.Failure("Review not found", 404);

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Success(true, 200);
        }
    }
}
