namespace Store.Shared.Dto
{
    public class OrderLineDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal NetCost { get; set; }

        public decimal VatCost { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = "Unknown";

        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; } = "Unknown";

        public virtual OrderDto? Order { get; set; } = null!;

        public virtual ProductDto? Product { get; set; } = null!;
    }
}
