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

        [Parameter]
        public OrderDto Order { get; set; }

        public List<SupplierDto> Suppliers { get; set; } = default!;

        public List<ProductDto> Products { get; set; } = default!;

        public bool IsNewSupplier { get; set; } = false;

        public ProductDto? NewProduct { get; set; }

        public string SearchValue { get; set; } = string.Empty;

        private IBrowserFile _SelectedFile;

        private List<string> _ImageUrisOrder = new List<string>();

        private string _ImageUriSupplier = "";

        private List<DropdownColumn> DropdownColumns = new List<DropdownColumn>();

        private List<Column> Columns = new List<Column>()
        {
            new Column
            {
                Name = "Product.Title"
            },
            new Column
            {
                Name = "Quantity"
            },
            new Column
            {
                Name = "Cost"
            },
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

        private void SetSelectedSupplier(SupplierDto supplier, bool isNewSupplier)
        {
            if (isNewSupplier)
            {
                Order.Supplier = new SupplierDto();
            }
            else
            {
                Order.Supplier = supplier;
            }
            IsNewSupplier = isNewSupplier;
        }

        private async Task OnInputFileSupplierChange(InputFileChangeEventArgs e)
        {
            var image = await e.File.RequestImageFileAsync("image/png", 600, 600);

            using Stream imageStream = image.OpenReadStream(1024 * 1024 * 10);

            using MemoryStream ms = new();
            //copy imageStream to Memory stream
            await imageStream.CopyToAsync(ms);

            //convert stream to base64
            _ImageUriSupplier = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
            StateHasChanged();
        }

        private async Task OnInputFileOrderChange(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles(e.FileCount))
            {
                var image = await file.RequestImageFileAsync("image/png", 600, 600);

                using Stream imageStream = image.OpenReadStream(1024 * 1024 * 10);

                using MemoryStream ms = new();
                //copy imageStream to Memory stream
                await imageStream.CopyToAsync(ms);

                //convert stream to base64
                _ImageUrisOrder.Add($"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}");
            }
            StateHasChanged();
        }

        private bool IsSupplierValied()
        {
            if (IsNewSupplier)
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
            else
            {
                if (Suppliers != null)
                {
                    if (Suppliers.Any(x => x.Id == Order.SupplierId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Orderlines
        private void DeleteItem(OrderLineDto orderLine)
        {
            Order.OrderLines.Remove(orderLine);
        }

        private void AddOrderLine(ProductDto product)
        {
            if (!Order.OrderLines.Any(x => x.ProductId == product.Id))
            {
                Order.OrderLines.Add(new OrderLineDto()
                {
                    Product = product,
                });
            }
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

        private void SubmitOrder()
        {
            Console.WriteLine("Submitted order ", Order);
            Order = null;
        }

        private void HandleKeyDown(KeyboardEventArgs e)
        {
            switch (e.Code)
            {
                case "Escape":
                    Order = null;
                    break;
            }
        }
    }
}
