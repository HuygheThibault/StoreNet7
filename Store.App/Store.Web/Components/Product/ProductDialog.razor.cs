using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Store.Shared.Dto;
using Store.Web.Exceptions;
using Store.Web.Models;
using Store.Web.Services;
using static Store.Web.Models.Noticiation;

namespace Store.Web.Components.Product
{
    public partial class ProductDialog
    {
        [Inject]
        public IProductService ProductService { get; set; } = default!;

        [Inject]
        public ICategoryService CategoryService { get; set; } = default!;

        [Inject]
        public NotificationService NotificationService { get; set; } = default!;

        [Parameter]
        public ProductDto? Product { get; set; }

        [Parameter]
        public EventCallback<ProductDto> OnResult { get; set; }

        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        protected bool IsNew = true;

        private EditContext EditContext;

        protected override async Task OnInitializedAsync()
        {
            Categories = (await CategoryService.GetAllCategorys()).ToList();
        }

        protected override async Task OnParametersSetAsync()
        {

            if (Product != null)
            {
                var dbModel = await ProductService.GetProductById(Product.Id);

                if (dbModel != null)
                {
                    Product = dbModel;
                    IsNew = false;
                }
                else
                {
                    IsNew = true;
                }

                EditContext = new EditContext(Product);
            }
        }

        private void CategoryChanged(CategoryDto category)
        {
            Product.Category = category;
        }

        private async Task Submit()
        {
            ProductDto savedProduct = null;

            if (EditContext.Validate())
            {
                try
                {
                    if (IsNew)
                    {
                        savedProduct = await ProductService.AddProduct(Product);
                    }
                    else
                    {
                        savedProduct = await ProductService.UpdateProduct(Product);
                    }
                }
                catch (HttpRequestFailedException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (savedProduct != null)
                {
                    NotificationService.ShowNotification(new Noticiation()
                    {
                        Name = $"{savedProduct.Title}: sucessfully saved",
                        Level = NoticiationLevel.Success
                    });
                    Product = null;
                    await OnResult.InvokeAsync(savedProduct);
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
            else
            {
                NotificationService.ShowNotification(new Noticiation()
                {
                    Name = $"{EditContext.GetValidationMessages().ToList()}",
                    Level = NoticiationLevel.Danger
                });
            }
        }
    }
}
