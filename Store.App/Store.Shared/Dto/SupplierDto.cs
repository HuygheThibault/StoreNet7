using Store.Shared.Modals;
using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Dto
{
    public class SupplierDto : BaseEntity
    {
        public Guid Id { get; set; } = new Guid();

        [Required, MinLength(3)]
        public string Name { get; set; }

        public string? Address { get; set; }

        //[Required, MinLength(16)]
        public string VatNumber { get; set; }
    }
}
