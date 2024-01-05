using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Shared.Modals;
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

        public List<ProductDto>? GridData { get; set; } = default!;

        public List<CategoryDto>? Categories { get; set; } = default!;

        public ProductDto? EditProduct { get; set; }

        List<DropdownColumn>? DropdownColumns = default!;

        public PaginationMetadata PaginationData = new PaginationMetadata();

        protected override async Task OnInitializedAsync()
        {
            await GetGridData();
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

        private async Task GetGridData()
        {

            if (PaginationData != null)
            {
                Tuple<IEnumerable<ProductDto>, PaginationMetadata> gridData;
                gridData = await ProductService.GetAllProducts(pageNumber: PaginationData.CurrentPage, pageSize: PaginationData.PageSize);

                if (gridData != null)
                {
                    GridData = gridData?.Item1.ToList();
                    PaginationData = gridData?.Item2;
                }
            }
            else
            {
                IEnumerable<ProductDto> gridData = await ProductService.GetAllProducts();
                if (gridData != null)
                {
                    GridData = gridData.ToList();
                }
            }
        }

        private async Task RefreshData()
        {
            await GetGridData();
            await InvokeAsync(StateHasChanged);
        }

        private void EditItem(ProductDto item)
        {
            EditProduct = item;
        }

        private void AddItem()
        {
            EditProduct = new ProductDto();
        }

        private async void ProductDialogResult(ProductDto product)
        {
            EditProduct = null;
            if (product != null)
            {
                await RefreshData();
            }
        }

        private async Task DeleteItem(ProductDto item)
        {
            await ProductService.DeleteProduct(item.Id);
            await RefreshData();
        }
    }
}