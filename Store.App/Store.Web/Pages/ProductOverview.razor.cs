using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Web.Helpers.Modals;
using Store.Web.Services;
using System.Data;

namespace Store.Web.Pages
{
    public partial class ProductOverview
    {
        [Inject]
        public IProductService ProductService { get; set; } = default!;

        [Inject]
        public ICategoryService CategoryService { get; set; } = default!;

        public List<ProductDto>? Items { get; set; } = default!;

        public List<CategoryDto>? Categories { get; set; } = default!;

        public ProductDto? EditProduct { get; set; }

        List<DropdownColumn>? DropdownColumns = default!;

        protected override async Task OnInitializedAsync()
        {
            Items = (await ProductService.GetAllProducts()).ToList();
            Categories = (await CategoryService.GetAllCategorys()).ToList();

            DropdownColumns = new List<DropdownColumn>()
            { new DropdownColumn()
            {
                Name = "CategoryId",
                    Values = Categories.Select(x => new DropdownOption()
                    {
                        Value = x.Title,
                        Id = x.Id,
                    }).ToList()
                }
            };
        }

        private async Task RefreshData()
        {
            Items = (await ProductService.GetAllProducts()).ToList();
        }

        private void EditItem(ProductDto item)
        {
            EditProduct = item;
        }

        private void AddItem()
        {
            EditProduct = new ProductDto();
        }

        private void ProductDialogResult(ProductDto product)
        {
            if (product != null)
            {
            }
            EditProduct = null;
        }

        private async Task DeleteItem(ProductDto item)
        {
            await ProductService.DeleteProduct(item.Id);
            await RefreshData();
        }
    }
}