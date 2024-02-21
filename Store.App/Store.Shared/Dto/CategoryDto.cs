using Store.Shared.Modals;

namespace Store.Shared.Dto
{
    public class CategoryDto : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }
    }
}
