using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;

namespace Store.Web.Components.Checkout
{
    public partial class TicketSidebar
    {
        public SaleDto Sale { get; set; } = new SaleDto();

        [Parameter]
        public SaleLineDto? NewSaleLine { get; set; } = default!;

        protected override void OnParametersSet()
        {
            if (NewSaleLine != null && NewSaleLine?.Product != null)
            {
                Sale.SaleLines.Add(NewSaleLine);
            }
        }
    }
}
