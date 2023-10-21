using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public string? FileName { get; set; }

        [Required]
        public decimal Cost { get; set; }

        public bool IsPaid { get; set; }

        [MaxLength(255)]
        public string? Comments { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public Guid SupplierId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int NumberOfOrderLines { get
            {
                return OrderLines.Count;
            } }

        public virtual ICollection<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();

        public virtual SupplierDto? Supplier { get; set; } = null!;
    }
}
