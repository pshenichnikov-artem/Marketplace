using AutoMapper;
using Marketplace.API;
using Marketplace.Core.DTOs.User;
using Marketplace.Core.Entities;
using Marketplace.Core.Exceptions;
using Marketplace.Core.Response;
using Marketplace.Service.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Marketplace.Service.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenGenerator _jwtGenerator;
    private readonly IMapper _mapper;

    public UserService(IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
    {
        _jwtGenerator = new JwtTokenGenerator(configuration);
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<string>> RegisterAsync(UserCreateRequest userCreateRequest)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userCreateRequest.Email);
        if (existingUser != null)
            return ServiceResult<string>.Failure("User already exists", 409);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(userCreateRequest.Password);
        var newUser = _mapper.Map<User>(userCreateRequest);
        newUser.PasswordHash = passwordHash;

        await _context.AddAsync(newUser);
        await _context.SaveChangesAsync();

        var token = _jwtGenerator.GenerateToken(newUser.Id.ToString(), newUser.Email, newUser.Role);
        return ServiceResult<string>.Success(token, 201);
    }

    public async Task<ServiceResult<string>> AuthenticateAsync(UserLoginRequest userData)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userData.Email);
        if (user == null)
            return ServiceResult<string>.Failure("User not found", 404);

        var token = _jwtGenerator.GenerateToken(user.Id.ToString(), user.Email, user.Role);
        return ServiceResult<string>.Success(token);
    }

    public async Task<ServiceResult<IEnumerable<AdminUserResponse>>> GetUsers(int page, int pageSize, string? search, string role)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(u =>
                u.Email.ToLower() == search ||
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Phone.Contains(search));
        }

        if (!string.IsNullOrEmpty(role))
            query = query.Where(u => u.Role == role);

        var users = await query
            .OrderByDescending(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mapped = _mapper.Map<IEnumerable<AdminUserResponse>>(users);
        return ServiceResult<IEnumerable<AdminUserResponse>>.Success(mapped);
    }

    public async Task<ServiceResult<UserSelfResponse>> GetUserById(long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return ServiceResult<UserSelfResponse>.Failure("User not found", 404);

        var mapped = _mapper.Map<UserSelfResponse>(user);
        return ServiceResult<UserSelfResponse>.Success(mapped);
    }

    public async Task<ServiceResult<bool>> UpdateUser(long id, UserUpdateRequest updateRequest)
    {
        var oldUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (oldUser == null)
            return ServiceResult<bool>.Failure("User not found", 404);

        oldUser.FirstName = updateRequest.FirstName;
        oldUser.LastName = updateRequest.LastName;
        oldUser.Address = updateRequest.Address;
        oldUser.Phone = updateRequest.Phone;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Success(true, 200);
    }

    public async Task<ServiceResult<bool>> DeleteUser(long id)
    {
        var oldUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (oldUser == null)
            return ServiceResult<bool>.Failure("User not found", 404);

        _context.Users.Remove(oldUser);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Success(true, 200);
    }

    public async Task<ServiceResult<bool>> ChangePassword(long userId, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return ServiceResult<bool>.Failure("User not found", 404);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordHash = passwordHash;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }
}