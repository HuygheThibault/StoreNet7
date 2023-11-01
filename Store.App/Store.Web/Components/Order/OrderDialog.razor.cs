using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Store.Shared.Dto;
using Store.Web.Exceptions;
using Store.Web.Models;
using Store.Web.Services;
using System.Reflection.Metadata;
using static Store.Web.Models.Noticiation;

namespace Store.Web.Components.Order
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

        private OrderDto? _order;

        [Parameter]
        public EventCallback<Noticiation> OnResult { get; set; }

        public List<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();

        private IBrowserFile selectedFile;

        protected bool IsSaving = false;

        protected bool IsNew = true;

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
                try
                {
                    var dbOrder = await OrderService.GetOrderById(_order.Id);
                    if (dbOrder != null)
                    {
                        _order = dbOrder;
                        IsNew = false;
                    }
                    else
                    {
                        IsNew = true;
                    }
                }
                catch (HttpRequestFailedException) { }
            }
            IsSaving = false;
        }

        private async Task Submit()
        {
            try
            {
                IsSaving = true;

                _order.ModifiedBy = "Admin";
                _order.ModifiedOn = DateTime.Now;
                _order.OrderLines.ToList().ForEach(x => x.ModifiedBy = "Admin");
                _order.OrderLines.ToList().ForEach(x => x.ModifiedOn = DateTime.Now);
                _order.OrderLines.Where(x => x.CreatedBy == null).ToList().ForEach(x => x.CreatedBy = "Admin");

                OrderDto savedOrder;

                if (IsNew)
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
                    _order.CreatedBy = "Admin";

                    savedOrder = await OrderService.AddOrder(_order);
                }
                else
                {
                    savedOrder = await OrderService.UpdateOrder(_order);
                }

                if (savedOrder != null)
                {
                    _order = null;
                    await OnResult.InvokeAsync(new Noticiation()
                    {
                        Name = "Order saved successfully",
                        Sort = NoticiationType.Success
                    });
                }
                else
                {
                    await OnResult.InvokeAsync(new Noticiation()
                    {
                        Name = "Error while saving order",
                        Sort = NoticiationType.Danger
                    });
                }
            }
            catch (Exception ex)
            {

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