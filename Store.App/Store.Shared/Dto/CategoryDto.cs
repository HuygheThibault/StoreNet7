namespace Store.Shared.Dto
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
