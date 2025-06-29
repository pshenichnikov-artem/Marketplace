using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.Core.Entities
{
    public class UserProductHistory
    {
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public long ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
