using System;
using System.Collections.Generic;

namespace Marketplace.Core.Entities;

public partial class Payment
{
    public long Id { get; set; }

    public long? OrderId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual Order? Order { get; set; }
}
