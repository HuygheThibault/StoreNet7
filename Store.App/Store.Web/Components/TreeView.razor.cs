using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Web.Services;

namespace Store.Web.Components
{
    public partial class TreeView
    {
        [Inject]
        public ICategoryService CategoryService { get; set; } = default!;

        [Parameter]
        public EventCallback<CategoryDto> CategorySelected { get; set; }

        public List<CategoryDto>? Categories { get; set; } = default!;

        private CategoryDto? _selectedCategory;

        protected override async Task OnInitializedAsync()
        {
            Categories = (await CategoryService.GetAllCategorys()).ToList();
        }

        private void CategoriesChanged(CategoryDto category)
        {
            _selectedCategory = category;
            CategorySelected.InvokeAsync(category);
        }
    }
}
