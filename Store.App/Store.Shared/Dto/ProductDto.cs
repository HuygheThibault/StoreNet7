using Store.Shared.Modals;
using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class ProductDto : BaseEntity
    {
        public Guid Id { get; set; } = new Guid();

        [Required, MinLength(3)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public decimal PricePerUnit { get; set; }

        public decimal QuantityInStock { get; set; }

        public byte[]? Image { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        private CategoryDto category;

        public virtual CategoryDto? Category
        {
            get { return category; }
            set { category = value; if (value != null) CategoryId = value.Id; }
        }
    }
}
