using Store.Shared.Modals;
using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class OrderLineDto : BaseEntity
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

        private ProductDto product;

        public virtual ProductDto? Product
        {
            get { return product; }
            set { product = value; if (value != null) ProductId = value.Id; }
        }
    }
}
