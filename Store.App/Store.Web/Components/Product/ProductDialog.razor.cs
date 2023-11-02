using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Store.Shared.Dto;
using Store.Web.Exceptions;
using Store.Web.Services;
using System.Collections.Generic;

namespace Store.Web.Components.Product
{
    public partial class ProductDialog
    {
        [Inject]
        public IProductService ProductService { get; set; } = default!;

        [Inject]
        public ICategoryService CategoryService { get; set; } = default!;

        [Parameter]
        public ProductDto? Product { get; set; }

        [Parameter]
        public EventCallback<ProductDto> Result { get; set; }

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
                if (IsNew)
                {
                    try
                    {
                        savedProduct = await ProductService.AddProduct(Product);
                    }
                    catch (HttpRequestFailedException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        savedProduct = await ProductService.UpdateProduct(Product);
                    }
                    catch (HttpRequestFailedException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (savedProduct != null)
                {
                    Product = savedProduct;
                }
                else
                {
                    Product = null;
                }

                await Result.InvokeAsync(Product);
            }
            else
            {
                Console.WriteLine("Form is Invalid...");
            }
        }

        private async Task Close()
        {
            Product = null;
            await Result.InvokeAsync(Product);
        }
    }
}
