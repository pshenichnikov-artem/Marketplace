using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Core.DTOs.Product
{
    public class ProductQueryParameters
    {
        public int Page { get; set; } = 0;
        public int? PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? OrderBy { get; set; }
        public int? CategoryId { get; set; }
        public string? SellerId { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }
}
