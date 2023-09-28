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

        private async Task SaveEdit(ProductDto item)
        {
            ProductDto updatedItem = await ProductService.UpdateProduct(item);

            await RefreshData();
        }

        //Save new item
        private async void SaveNewItem(ProductDto item)
        {
            if (item.Id != Guid.Empty)
            {
                ProductDto newItem = await ProductService.AddProduct(item);

                await RefreshData();
            }
        }

        //DELETE
        private async Task DeleteItem(Tuple<ProductDto, bool> item)
        {
            await ProductService.DeleteProduct(item.Item1.Id);

            await RefreshData();
        }
    }
}