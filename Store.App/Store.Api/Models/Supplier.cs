using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Supplier
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? VatNumber { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
