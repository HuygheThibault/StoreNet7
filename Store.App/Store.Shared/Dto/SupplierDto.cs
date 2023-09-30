namespace Store.Shared.Dto
{
    public class SupplierDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? VatNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
