using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Store.Shared.Dto;
using Store.Web.Models;
using Store.Web.Services;
using System.Reflection.Metadata;
using static Store.Web.Models.Noticiation;

namespace Store.Web.Components.Dialog
{
    public partial class OrderDialog
    {
        [Inject]
        public IOrderService OrderService { get; set; } = default!;

        [Inject]
        public ISupplierService SupplierService { get; set; } = default!;

        [Inject]
        public IProductService ProductService { get; set; } = default!;

        [Parameter]
        public OrderDto? Order { get; set; }

        [Parameter]
        public EventCallback<Noticiation> OnResult { get; set; }

        private OrderDto? _order;

        public List<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();

        private IBrowserFile selectedFile;

        protected bool IsSaving = false;

        protected override async Task OnInitializedAsync()
        {
            Suppliers = (await SupplierService.GetAllSuppliers()).ToList();
            Products = (await ProductService.GetAllProducts()).ToList();
        }

        protected override async Task OnParametersSetAsync()
        {
            _order = Order;

            if (_order != null)
            {
                if (_order?.Id != Guid.Empty)
                {
                    _order = await OrderService.GetOrderById(_order.Id);
                }
            }
            IsSaving = false;
        }

        private async Task Submit()
        {
            IsSaving = true;

            if (_order.Id == Guid.Empty)
            {
                if (selectedFile != null)
                {
                    var file = selectedFile;
                    Stream stream = file.OpenReadStream();
                    MemoryStream ms = new();
                    await stream.CopyToAsync(ms);
                    stream.Close();

                    _order.FileName = file.Name;
                }

                _order.OrderLines.ToList().ForEach(x => x.OrderId = _order.Id);

                var addedOrder = await OrderService.AddOrder(_order);
                _order = null;
                await OnResult.InvokeAsync(new Noticiation()
                {
                    Name = "Order added",
                    Sort = NoticiationType.Success
                });
            }
            else
            {
                var addedOrder = await OrderService.UpdateOrder(_order);
                _order = null;
                await OnResult.InvokeAsync(new Noticiation()
                {
                    Name = "Order updated",
                    Sort = NoticiationType.Success
                });
            }
        }

        private async Task Cancel()
        {
            _order = null;
            await OnResult.InvokeAsync(new Noticiation()
            {
                Name = "Order canceled",
                Sort = NoticiationType.Danger
            });
        }

        private void AddOrderLine()
        {
            _order.OrderLines.Add(new OrderLineDto());
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            StateHasChanged();
        }
    }
}