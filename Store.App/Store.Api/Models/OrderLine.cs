using Store.Shared.Modals;

namespace Store.Api.Models;

public partial class OrderLine: BaseEntity
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal? CostPerItem { get; set; }

    public decimal? Cost { get; set; }

    public Guid OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
