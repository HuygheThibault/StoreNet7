using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Store.Shared.Dto;
using Store.Web.Services;
using System.Reflection.Metadata;

namespace Store.Web.Pages.Components.Dialog
{
    public partial class OrderDialog
    {
        [Parameter]
        public OrderDto? Order { get; set; }

        private OrderDto? _order;

        public List<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();

        [Inject]
        public IOrderService OrderService { get; set; } = default!;

        [Inject]
        public ISupplierService SupplierService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Suppliers = (await SupplierService.GetAllSuppliers()).ToList();
        }

        protected override async Task OnParametersSetAsync()
        {
            _order = Order;

            if (_order != null)
            {
                if (_order?.Id != Guid.Empty)
                {
                    Order = await OrderService.GetOrderById(_order.Id);
                }
                else
                {
                    _order = new OrderDto();
                }
            }
        }

        private async Task Submit(EditContext editContext)
        {
            if (editContext.Validate())
            {
                Console.WriteLine("valid submit");
                _order = null;
            }
        }

        private void Cancel()
        {
            _order = null;
        }
    }
}
