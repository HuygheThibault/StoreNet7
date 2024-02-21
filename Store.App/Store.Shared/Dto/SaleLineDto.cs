using Store.Shared.Modals;

namespace Store.Shared.Dto
{
    public class SaleLineDto : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid SaleId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal NetCost { get; set; }

        public decimal VatCost { get; set; }

        public virtual ProductDto Product { get; set; } = null!;
    }
}
