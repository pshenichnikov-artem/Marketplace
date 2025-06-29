using System;
using System.Collections.Generic;
namespace Marketplace.Core.DTOs.Cart
{
    public class CartItemUpdateRequest
    {
        public long CartItemId { get; set; }
        public short Quantity { get; set; }
    }
}
