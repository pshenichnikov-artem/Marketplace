using System;
using System.Collections.Generic;

namespace Marketplace.Core.Entities;

public partial class OrderItem
{
    public long Id { get; set; }

    public long? Productid { get; set; }

    public long? OrderId { get; set; }

    public decimal Price { get; set; }

    public short Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
