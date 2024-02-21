using Store.Shared.Modals;

namespace Store.Api.Models;

public partial class Order: BaseEntity
{
    public Guid SupplierId { get; set; }

    public string? FileName { get; set; }

    public decimal Cost { get; set; }

    public bool IsPaid { get; set; }

    public string? Comments { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Supplier Supplier { get; set; } = null!;
}
