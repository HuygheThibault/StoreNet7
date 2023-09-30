using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Sale
{
    public Guid Id { get; set; }

    public string? FileName { get; set; }

    public decimal? TotalCost { get; set; }

    public decimal? TotalVastCost { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual ICollection<SaleLine> SaleLines { get; set; } = new List<SaleLine>();
}
