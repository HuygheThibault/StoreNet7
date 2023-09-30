using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class DiscountType
{
    public Guid Id { get; set; }

    public string Condition { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
