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
                if (Sale.SaleLines.Select(x => x.Product.Id).ToList().Contains(NewSaleLine.Product.Id))
                {
                    AddQuantity(NewSaleLine);
                }
                else
                {
                    Sale.SaleLines.Add(NewSaleLine);
                }
            }
        }

        private void AddQuantity(SaleLineDto newSaleLine)
        {
            if (NewSaleLine != null)
            {
                SaleLineDto saleLine = Sale?.SaleLines?.FirstOrDefault(x => x.Product.Id == newSaleLine.Product.Id);
                saleLine.Quantity++;
            }
        }

        private void RemoveQuantity(SaleLineDto newSaleLine)
        {
            if (NewSaleLine != null)
            {
                SaleLineDto saleLine = Sale?.SaleLines?.FirstOrDefault(x => x.Product.Id == newSaleLine.Product.Id);

                if (saleLine.Quantity > 1)
                {
                    saleLine.Quantity--;
                }
            }
        }

        private void Remove(SaleLineDto newSaleLine)
        {
            if (NewSaleLine != null)
            {
                SaleLineDto saleLine = Sale?.SaleLines?.FirstOrDefault(x => x.Product.Id == newSaleLine.Product.Id);
                Sale.SaleLines.Remove(saleLine);

            }
        }
    }
}
