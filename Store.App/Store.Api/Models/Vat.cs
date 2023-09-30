using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Vat
{
    public Guid Id { get; set; }

    public int Value { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
