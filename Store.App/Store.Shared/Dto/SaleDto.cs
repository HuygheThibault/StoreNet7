namespace Store.Shared.Dto
{
    public class SaleDto
    {
        public Guid Id { get; set; }

        public string? FileName { get; set; }

        public decimal? TotalCost { get; set; }

        public decimal? TotalVastCost { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; } = null!;

        public virtual ICollection<SaleLineDto> SaleLines { get; set; } = new List<SaleLineDto>();
    }
}
