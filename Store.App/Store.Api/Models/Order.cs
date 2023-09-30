using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public string? FileName { get; set; }

    public decimal TotalCost { get; set; }

    public decimal TotalVatCost { get; set; }

    public bool IsPaid { get; set; }

    public Guid SupplierId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string Createdby { get; set; } = null!;

    public DateTime UpdatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Supplier Supplier { get; set; } = null!;
}
