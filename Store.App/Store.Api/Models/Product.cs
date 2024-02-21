using Store.Shared.Modals;

namespace Store.Api.Models;

public partial class Product: BaseEntity
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? PricePerUnit { get; set; }

    public byte[]? Image { get; set; }

    public int? QuantityInStock { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? VatId { get; set; }

    public Guid? DiscountId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual ICollection<SaleLine> SaleLines { get; set; } = new List<SaleLine>();

    public virtual Vat? Vat { get; set; }
}
