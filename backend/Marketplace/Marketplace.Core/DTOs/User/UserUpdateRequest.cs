namespace Marketplace.Core.DTOs.User
{
    public class UserUpdateRequest
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Phone { get; set; } = null!;
        public string? Address { get; set; } = null!;
    }
}
