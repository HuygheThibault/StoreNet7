using Store.Shared.Modals;

namespace Store.Shared.Dto
{
    public class SaleDto : BaseEntity
    {
        public Guid Id { get; set; }

        public string? FileName { get; set; }

        public decimal? TotalCost { get; set; }

        public decimal? TotalVastCost { get; set; }

        public virtual ICollection<SaleLineDto> SaleLines { get; set; } = new List<SaleLineDto>();
    }
}
