using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Store.Shared.Dto;
using Store.Web.Helpers.Modals;
using Store.Web.Services;

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

        public List<SupplierDto> Suppliers { get; set; } = default!;

        public List<ProductDto> Products { get; set; } = default!;

        public bool IsWizardVisible { get; set; } = true;

        public OrderDto Order { get; set; } = new OrderDto() { Supplier = new SupplierDto(), OrderLines = new List<OrderLineDto>() };

        public bool IsNewSupplier { get; set; } = false;

        public ProductDto NewProduct { get; set; } = default!;

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
                Name = "Product.Title"
            }
            ,
            new Column
            {
                Name = "Actions"
            }
        };

        protected override async Task OnInitializedAsync()
        {
            Suppliers = (await SupplierService.GetAllSuppliers()).ToList();
            Products = (await ProductService.GetAllProducts()).ToList();
        }

        private void SetSelectedSupplier(SupplierDto supplier, bool? isNewSupplier = null)
        {
            if (isNewSupplier.HasValue)
            {
                IsNewSupplier = isNewSupplier.Value;
                Order.Supplier = new SupplierDto();
            }
            else
            {
                Order.Supplier = supplier;
            }
        }

        private void OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
        }

        private bool IsSupplierValied()
        {
            if (Suppliers != null)
            {
                if (Suppliers.Any(x => x.Id == Order.SupplierId))
                {
                    return true;
                }

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
            return false;
        }

        // Orderlines
        private void DeleteItem(OrderLineDto orderLine)
        {
            Order.OrderLines.Remove(orderLine);
        }

        private void Edit(OrderLineDto orderLine)
        {
        }

        private void AddOrderLine(ProductDto product)
        {
            Order.OrderLines.Add(new OrderLineDto()
            {
                Product = product,
            });
        }

        private void AddNewProduct()
        {
            NewProduct = new ProductDto() { };
        }

        private void ProductDialogResult(ProductDto product)
        {
            if (product != null)
            {
                AddOrderLine(product);
            }
            NewProduct = null;
        }

        private void HandleKeyDown(KeyboardEventArgs e)
        {
            switch (e.Code)
            {
                case "Escape":
                    IsWizardVisible = false;
                    break;
            }
        }
    }
}
