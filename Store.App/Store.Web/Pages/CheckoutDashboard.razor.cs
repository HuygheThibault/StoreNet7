using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Web.Helpers.Modals;
using Store.Web.Services;

namespace Store.Web.Pages
{
    public partial class CheckoutDashboard
    {
        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]

        public ICategoryService CategoryService { get; set; }

        private List<ProductDto>? _Products = new List<ProductDto>();

        private List<CategoryDto>? _Categories = new List<CategoryDto>();

        private CategoryDto _SelectedCategory = default!;

        private CategoryDto _SelectedSubCategory = default!;

        private SaleLineDto? _SaleLine = default!;

        protected override async Task OnInitializedAsync()
        {
            _Products = (await ProductService.GetAllProducts()).ToList();
            _Categories = (await CategoryService.GetAllCategorys()).ToList();

            _SelectedCategory = _Categories.First();
        }

        private void SetSelectedCategroy(CategoryDto category)
        {
            _SelectedCategory = category;
        }

        private void SetSelectedSubCategroy(CategoryDto category)
        {
            _SelectedSubCategory = category;
        }

        private void AddItemToTicket(ProductDto product)
        {
           _SaleLine = new SaleLineDto() { Id = Guid.NewGuid(), Product = product, Quantity = 1 };
        }
    }
}
