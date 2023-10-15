using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;

namespace Store.Web.Components.Checkout
{
    public partial class ProductCard
    {
        [Parameter]
        public ProductDto Product { get; set; } = default!;
    }
}
