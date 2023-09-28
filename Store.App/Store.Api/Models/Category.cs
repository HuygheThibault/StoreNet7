using System;
using System.Collections.Generic;

namespace Store.Api.Models;

public partial class Category
{
    public Guid Id { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Color { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
