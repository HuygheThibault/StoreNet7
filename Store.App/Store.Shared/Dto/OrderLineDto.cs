namespace Store.Shared.Dto
{
    public class OrderLineDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal? CostPerItem { get; set; }

        public decimal? Cost { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public virtual ProductDto? Product { get; set; } = null!;
    }
}
