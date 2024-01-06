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

        [Inject]
        public NotificationService NotificationService { get; set; } = default!;

        [Parameter]
        public OrderDto? Order { get; set; }

        private OrderDto? _order;

        [Parameter]
        public EventCallback<OrderDto> OnResult { get; set; }

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
                try
                {
                    var dbOrder = await OrderService.GetOrderById(_order.Id);
                    if (dbOrder != null)
                    {
                        _order = dbOrder;
                    }
                }
                catch (HttpRequestFailedException) { }
            }
            IsSaving = false;
        }

        public async Task Submit()
        {
            try
            {
                IsSaving = true;

                OrderDto savedOrder = await OrderService.UpdateOrder(_order);

                if (savedOrder != null)
                {
                    NotificationService.ShowNotification(new Noticiation()
                    {
                        Name = $"{_order.FileName}: sucessfully saved",
                        Level = NoticiationLevel.Success
                    });
                    _order = null;
                    await OnResult.InvokeAsync(savedOrder);
                }
                else
                {
                    NotificationService.ShowNotification(new Noticiation()
                    {
                        Name = $"Error while updating",
                        Level = NoticiationLevel.Danger
                    });
                }
            }
            catch (Exception ex)
            {
                NotificationService.ShowNotification(new Noticiation()
                {
                    Name = $"{ex.Message}",
                    Level = NoticiationLevel.Danger
                });
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void AddOrderLine()
        {
            if(_order != null)
                _order.OrderLines.Add(new OrderLineDto());
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            StateHasChanged();
        }
    }
}