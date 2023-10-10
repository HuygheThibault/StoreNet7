using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Web.Services;

namespace Store.Web.Pages
{
    public partial class SupplierOverview
    {
        [Inject]
        public ISupplierService SupplierService { get; set; }

        private List<SupplierDto> _suppliers = new List<SupplierDto>();

        protected override async Task OnInitializedAsync()
        {
            _suppliers = (await SupplierService.GetAllSuppliers()).ToList();
        }
    }
}
