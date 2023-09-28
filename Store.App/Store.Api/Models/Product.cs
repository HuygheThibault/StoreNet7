using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? PricePerUnit { get; set; }

    public byte[]? Image { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
