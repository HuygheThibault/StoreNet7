using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Store.Shared.Dto;
using Store.Web.Helpers.Modals;
using Store.Web.Models;
using Store.Web.Services;
using static Store.Web.Models.Noticiation;

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

        [Inject]
        public NotificationService NotificationService { get; set; } = default!;

        [Parameter]
        public OrderDto Order { get; set; }

        [Parameter]
        public EventCallback<OrderDto> OnResult { get; set; }

        public List<SupplierDto> Suppliers { get; set; } = default!;

        public List<ProductDto> Products { get; set; } = default!;

        public bool IsNewSupplier { get; set; } = false;

        public ProductDto? NewProduct { get; set; }

        public string SearchValue { get; set; } = string.Empty;

        private IBrowserFile _SelectedFile;

        private List<string> _ImageUrisOrder = new List<string>();

        private string _ImageUriSupplier = "";

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
                Name = "CostPerItem"
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

        private void SetSelectedSupplier(SupplierDto supplier)
        {
            Order.Supplier = supplier;
            IsNewSupplier = false;
        }

        private void SetNewSupplier(bool isNewSupplier)
        {
            IsNewSupplier = isNewSupplier;

            if (IsNewSupplier)
            {
                Order.Supplier = new SupplierDto();
            }
            else
            {
                Order.Supplier = null;
            }
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
            if (Order != null && IsNewSupplier)
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
        private void DeleteItemFromProductLines(OrderLineDto orderLine)
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
            NewProduct = null;
            if (product != null)
            {
                AddOrderLine(product);
            }
        }

        private async Task SubmitOrder()
        {
            if (Order != null)
            {
                if (Order.Supplier != null)
                {
                    if (IsNewSupplier)
                    {
                        Order.Supplier = await SupplierService.AddSupplier(Order.Supplier);
                     }

                    Order = await OrderService.AddOrder(Order);

                    if (Order != null)
                    {
                        NotificationService.ShowNotification(new Noticiation()
                        {
                            Name = $"{Order.FileName}: sucessfully saved new order",
                            Level = NoticiationLevel.Success
                        });
                        await OnResult.InvokeAsync(Order);
                    }
                    else
                    {
                        NotificationService.ShowNotification(new Noticiation()
                        {
                            Name = $"Error while saving order",
                            Level = NoticiationLevel.Danger
                        });
                    }
                }
                else
                    NotificationService.ShowNotification(new Noticiation()
                    {
                        Name = $"Error with the supplier",
                        Level = NoticiationLevel.Danger
                    });
            }
        }
    }
}
