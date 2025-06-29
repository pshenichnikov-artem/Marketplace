﻿using System;
using System.Collections.Generic;

namespace Marketplace.Core.Entities;

public partial class Category
{
    public long Id { get; set; }

    public string? CategoryName { get; set; }

    public long? ParentCategoryId { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
