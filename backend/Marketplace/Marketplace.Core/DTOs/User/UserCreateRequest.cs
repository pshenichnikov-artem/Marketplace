namespace Marketplace.Core.DTOs.User
{
    public class UserCreateRequest
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Role { get; set; }

        public string? Address { get; set; }
    }
}
