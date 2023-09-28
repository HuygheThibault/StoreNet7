using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Ticket
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
