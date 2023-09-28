using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Discount
{
    public Guid Id { get; set; }

    public DateTime? ActiveFrom { get; set; }

    public DateTime? ActiveUntil { get; set; }

    public int? AmountToBuy { get; set; }

    public int? AmountCharged { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;
}
