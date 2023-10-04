using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Shared.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public string? FileName { get; set; }

        [Required]
        public decimal TotalCost { get; set; }

        public decimal TotalVatCost { get; set; }

        public bool IsPaid { get; set; }

        public Guid SupplierId { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = "Unknown";

        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; } = "Unknown";

        public virtual ICollection<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();

        public virtual SupplierDto? Supplier { get; set; } = null!;
    }
}
