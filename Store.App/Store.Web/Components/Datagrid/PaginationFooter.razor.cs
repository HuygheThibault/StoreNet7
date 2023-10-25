using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;

namespace Store.Web.Components.Datagrid
{
    public partial class PaginationFooter
    {
        [Parameter]
        public virtual PaginationMetadata? Pagination { get; set; } = default!;

        [Parameter]
        public EventCallback<PaginationMetadata> ValueChanged { get; set; }

        private void IncrementPage()
        {
            if (Pagination.CurrentPage < Pagination.TotalPageCount)
            {
                Pagination.CurrentPage++;
                ValueChanged.InvokeAsync(Pagination);
            }
        }

        private void DecrementPage()
        {
            if (Pagination.CurrentPage > 1)
            {
                Pagination.CurrentPage--;
                ValueChanged.InvokeAsync(Pagination);
            }
        }

        private void PageSizeChanged(ChangeEventArgs args)
        {
            Pagination.PageSize = int.Parse(args.Value.ToString());
            ValueChanged.InvokeAsync(Pagination);
        }
    }
}
