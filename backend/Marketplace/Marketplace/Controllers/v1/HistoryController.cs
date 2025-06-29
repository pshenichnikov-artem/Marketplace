using AutoMapper;
using Marketplace.API.Attributes;
using Marketplace.Core.DTOs.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Marketplace.API.Controllers.v1
{
    public class HistoryController : CustomControllerBase
    {
        public ApplicationDbContext _context;
        public IMapper _mapper;

        public HistoryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ValidateToken]
        public async Task<IActionResult> GetHistory()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return ErrorResponse("Authorization header is missing or invalid", 401);
            }

            var token = authorizationHeader.Replace("Bearer ", "");

            long userId;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {

                }

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out userId))
                {
                    return ErrorResponse("Invalid token", 401);
                }
            }
            catch (Exception ex)
            {
                return ErrorResponse("Invalid token", 401);
            }

            var result = await _context.UserProductHistories
                .Where(u => u.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Include(u => u.Product)
                .Include(p => p.Product.ProductImages)
                .Include(p => p.Product.Reviews)
                .Include(p => p.Product.Seller)
                .Take(15)
                .ToListAsync();

            var mapProducts = _mapper.Map<IEnumerable<ProductResponse>>(result.Select(p => p.Product));


            return OkResponse(mapProducts);
        }
    }
}
