using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Store.Shared.Dto;
using Store.Web.Helpers.Modals;
using Store.Web.Services;
using System.Collections.Generic;

namespace Store.Web.Components.Order
{
    public partial class OrderWizard
    {
        [Inject]
        public IOrderService OrderService { get; set; } = default!;

        [Inject]
        public ISupplierService SupplierService { get; set; } = default!;

        [Inject]
        public IProductService ProductService { get; set; } = default!;

        public List<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();

        public bool IsVisible { get; set; } = true;

        public OrderDto Order { get; set; } = new OrderDto() { Supplier = new SupplierDto(), OrderLines = new List<OrderLineDto>() };

        public bool IsNewSupplier { get; set; } = false;

        public string SearchValue { get; set; } = string.Empty;

        private IBrowserFile selectedFile;

        private List<DropdownColumn> DropdownColumns = new List<DropdownColumn>();

        private List<Column> Columns = new List<Column>()
        {
            new Column
            {
                Name = "Quantity"
            },
            new Column
            {
                Name = "Product"
            }
        };

        protected override async Task OnInitializedAsync()
        {
            Suppliers = (await SupplierService.GetAllSuppliers()).ToList();
            Products = (await ProductService.GetAllProducts()).ToList();
        }

        private void SetSelectedSupplier(SupplierDto supplier)
        {
            Order.Supplier = supplier;
            StateHasChanged();
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            StateHasChanged();
        }

        private bool IsSupplierValied()
        {
            if (Order.Supplier != null && Order.Supplier.Name != null && Order.Supplier.VatNumber != null)
            {
                if (Order.Supplier.Name.Length < 5)
                {
                    return false;
                }
                else if (Order.Supplier.VatNumber.Length < 1)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        // Orderlines

        private void DeleteItem(OrderLineDto orderLine)
        {
            Order.OrderLines.Remove(orderLine);
            StateHasChanged();
        }

        private void Edit(OrderLineDto orderLine)
        {
            StateHasChanged();
        }

        private void AddOrderLine()
        {
            Order.OrderLines.Add(new OrderLineDto()
            {
                ProductId = Products.First().Id,

            });
            StateHasChanged();
        }

        private void HandleKeyDown(KeyboardEventArgs e)
        {
            switch (e.Code)
            {
                case "Escape":
                    IsVisible = false;
                    break;
            }
        }

        private static object GetPropValue(object src, string propName)
        {
            return src?.GetType()?.GetProperty(propName)?.GetValue(src, null);
        }
    }
}
