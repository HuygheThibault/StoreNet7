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

        [Required]
        public string? FileName { get; set; }

        [Required]
        public decimal TotalCost { get; set; }

        public decimal TotalVatCost { get; set; }

        public bool IsPaid { get; set; }

        public Guid SupplierId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Createdby { get; set; } = null!;

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; } = null!;

        public virtual ICollection<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();

        public virtual SupplierDto Supplier { get; set; } = null!;
    }
}
