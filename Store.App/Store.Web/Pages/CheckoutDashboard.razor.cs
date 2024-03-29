﻿using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Web.Helpers.Modals;
using Store.Web.Services;
using System.Diagnostics;

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
            _SelectedSubCategory = null;
        }

        private void SetSelectedSubCategroy(CategoryDto category)
        {
            _SelectedSubCategory = category;
        }

        private void AddItemToTicket(ProductDto product)
        {
           _SaleLine = new SaleLineDto() { Id = Guid.NewGuid(), Product = product, Quantity = 1 };
        }



        public List<ProductDto> GetAllProductsUnderTopCategory()
        {
            if(_SelectedSubCategory != null)
            {
                return GetAllProductsInCategoryAndChildren(_SelectedSubCategory.Id);
            }
            else if(_SelectedCategory != null)
            {
                return GetAllProductsInCategoryAndChildren(_SelectedCategory.Id);
            }
            else
            {
                return _Products;
            }
        }

        public List<CategoryDto> GetAllChildCategories(Guid parentId)
        {
            var childCategories = new List<CategoryDto>();

            // Find all categories where the ParentId matches the provided parentId
            var directChildren = _Categories.Where(c => c.ParentCategoryId == parentId).ToList();

            // Add direct children and recursively find their children
            foreach (var child in directChildren)
            {
                childCategories.Add(child);
                childCategories.AddRange(GetAllChildCategories(child.Id));
            }

            return childCategories;
        }

        public List<ProductDto> GetAllProductsInCategoryAndChildren(Guid categoryId)
        {
            var categories = GetAllChildCategories(categoryId).Select(c => c.Id).ToList();

            var products = _Products.Where(p => categories.Contains(p.CategoryId));

            return products.ToList();
        }
    }
}
