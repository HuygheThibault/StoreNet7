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
            ProductDto savedProduct;

            if (EditContext.Validate())
            {
                if (IsNew)
                {
                    savedProduct = await ProductService.AddProduct(Product);
                }
                else
                {
                    savedProduct = await ProductService.UpdateProduct(Product);
                }

                if (savedProduct != null)
                {
                    Product = null;
                }
            }
            else
            {
                Console.WriteLine("Form is Invalid...");
            }
        }

        private void Close()
        {
            Product = null;
        }
    }
}
