using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Web.Services;

namespace Store.Web.Pages
{
    public partial class OrderOverview
    {
        [Inject]
        public IOrderService OrderService { get; set; }

        [Inject]
        public ISupplierService SupplierService { get; set; }

        private OrderDto? _order;

        private void AddOrder()
        {
            _order = new OrderDto();
        }
    }
}
