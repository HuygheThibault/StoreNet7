using Store.Shared.Modals;
using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class ProductDto : DatagridRows
    {
        public Guid Id { get; set; }

        [Required, MinLength(3)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public decimal PricePerUnit { get; set; }

        public decimal QuantityInStock { get; set; }

        public byte[]? Image { get; set; }

        public Guid CategoryId { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = "Unknown";

        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; } = "Unknown";

        private CategoryDto category;

        public virtual CategoryDto? Category
        {
            get { return category; }
            set { category = value; if (value != null) CategoryId = value.Id; }
        }
    }
}
