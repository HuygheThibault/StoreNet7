﻿using Store.Shared.Modals;
using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class OrderDto : BaseEntity
    {
        //public Guid Id { get; set; } = Guid.NewGuid();

        public string? FileName { get; set; }

        [Required]
        public decimal Cost { get { return this.OrderLines.Sum(x => x.Cost).Value; } }

        public bool IsPaid { get; set; }

        [MaxLength(255)]
        public string? Comments { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public Guid SupplierId { get; set; }

        public int NumberOfOrderLines
        {
            get
            {
                return OrderLines.Count;
            }
        }

        public virtual ICollection<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();

        private SupplierDto supplier;

        public virtual SupplierDto? Supplier
        {
            get { return supplier; }
            set { supplier = value; if (value != null) SupplierId = value.Id; }
        }
    }
}
