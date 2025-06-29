using System;
using System.Collections.Generic;

namespace Marketplace.Core.Entities;

public partial class ProductImage
{
    public long Id { get; set; }

    public long? Productid { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product? Product { get; set; }
}
