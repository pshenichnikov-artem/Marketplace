namespace Marketplace.Core.Entities;

public partial class CartItem
{
    public long Id { get; set; }

    public long? CartId { get; set; }

    public long? ProductId { get; set; }

    public short Quantity { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Product? Product { get; set; }
}
