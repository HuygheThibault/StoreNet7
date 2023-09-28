using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Store.Shared.Modals;
using Store.Web.Helpers.Modals;
using System.Reflection;

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

        private JsonPatchDocument _patchDoc = new JsonPatchDocument();

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

        private void UpdateProp(PropertyInfo p, object value, string type)
        {
            switch (type)
            {
                case "String":
                    value = value.ToString();
                    break;
                case "Int32":
                    value = int.Parse(value.ToString());
                    break;
                case "Decimal":
                    value = Decimal.Parse(value.ToString());
                    break;
                case "DateTime":
                    value = DateTime.Parse(value.ToString());
                    break;
            }
            p.SetValue(EditContextItem, value);
            _patchDoc.Replace(p.Name, value);
        }
    }
}
