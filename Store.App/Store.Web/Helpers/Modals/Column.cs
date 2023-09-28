using Store.Shared.Enums;

namespace Store.Web.Helpers.Modals
{
    public class Column
    {
        public string Name { get; set; }

        public string? ColumnType { get; set; }

        public DropdownColumn? Dropdown { get; set; }

        public bool IsDisabled { get; set; } = false;

        public SortDirection Sort { get; set; } = SortDirection.None;

        public int SortOrder { get; set; } = -1;
    }
}
