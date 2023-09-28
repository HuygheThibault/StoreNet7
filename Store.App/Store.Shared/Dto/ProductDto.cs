namespace Store.Shared.Dto
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = String.Empty;

        public string? Description { get; set; }

        public decimal PricePerUnit { get; set; }

        public byte[]? Image { get; set; }

        public Guid CategoryId { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = "Unknown";

        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; } = "Unknown";

        public int Quantity { get; set; }
    }
}
