using Store.Shared.Modals;

namespace Store.Api.Models;

public partial class Category: BaseEntity
{
    public Guid? ParentCategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
