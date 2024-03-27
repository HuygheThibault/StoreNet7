namespace Store.Shared.Modals
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
