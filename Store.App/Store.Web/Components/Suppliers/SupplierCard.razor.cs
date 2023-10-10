using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;

namespace Store.Web.Components.Suppliers
{
    public partial class SupplierCard
    {
        [Parameter]
        public SupplierDto Supplier { get; set; } = default!;
    }
}
