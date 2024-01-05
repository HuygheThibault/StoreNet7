using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class SupplierDto
    {
        public Guid Id { get; set; } = new Guid();

        [Required, MinLength(3)]
        public string Name { get; set; }

        public string? Address { get; set; }

        //[Required, MinLength(16)]
        public string VatNumber { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;

        public string? ModifiedBy { get; set; }
    }
}
