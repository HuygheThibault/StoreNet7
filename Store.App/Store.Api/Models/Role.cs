using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
