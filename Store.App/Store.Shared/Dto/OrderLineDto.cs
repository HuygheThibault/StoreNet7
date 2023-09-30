namespace Store.Shared.Dto
{
    public class OrderLineDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal NetCost { get; set; }

        public decimal VatCost { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; } = null!;

        public virtual OrderDto Order { get; set; } = null!;

        public virtual ProductDto Product { get; set; } = null!;
    }
}
