using Store.Shared.Modals;
using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class OrderLineDto : DatagridRows
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }


        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal? CostPerItem { get; set; }

        public decimal? Cost { get { return this.CostPerItem * this.Quantity; } }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; } = string.Empty;

        private ProductDto product;

        public virtual ProductDto? Product
        {
            get { return product; }
            set { product = value; if (value != null) ProductId = value.Id; }
        }
    }
}
