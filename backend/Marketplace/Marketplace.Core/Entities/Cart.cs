using System;
using System.Collections.Generic;

namespace Marketplace.Core.Entities;

public partial class Cart
{
    public long Id { get; set; }

    public long? Userid { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
