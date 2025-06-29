using System;
using System.Collections.Generic;

namespace Marketplace.Core.Entities;

public partial class Review
{
    public long Id { get; set; }

    public long? ProductId { get; set; }

    public long? UserId { get; set; }

    public short? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
