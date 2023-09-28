using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Store.Shared.Modals;
using Store.Web.Helpers.Modals;

namespace Store.Web.Pages.Components.Datagrid
{
    public partial class DatagridEdit<T> where T : DatagridRows
    {
        [Parameter]
        public T? Item { get; set; } = default(T);

        private T? _item { get; set; }

        public T EditContextItem { get; set; } = default;

        [Parameter]
        public List<string> ColumnsToIgnore { get; set; }

        [Parameter]
        public List<Column> Columns { get; set; }

        [Parameter]
        public EventCallback<T> OnSave { get; set; }

        private JsonPatchDocument<T> _patchDoc = new JsonPatchDocument<T>();

        protected override void OnParametersSet()
        {
            _item = Item;

            EditContextItem = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(Item));
        }

        private async Task SaveEdit()
        {
            await OnSave.InvokeAsync(EditContextItem);
        }

        private async Task Close()
        {
            await OnSave.InvokeAsync(_item);
        }
    }
}
