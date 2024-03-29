﻿using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Discount
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTime? ActiveFrom { get; set; }

    public DateTime? ActiveUntil { get; set; }

    public Guid? DiscountTypeId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual DiscountType? DiscountType { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
