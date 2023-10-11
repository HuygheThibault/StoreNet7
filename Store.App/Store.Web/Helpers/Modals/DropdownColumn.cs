namespace Store.Web.Helpers.Modals
{
    public class DropdownColumn
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
        public List<DropdownOption> Values { get; set; } = new List<DropdownOption>();
    }
}
