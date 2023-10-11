using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Store.Shared.Enums;
using Store.Shared.Modals;
using Store.Web.Helpers.Modals;
using System.Reflection;

namespace Store.Web.Components.Datagrid
{
    public partial class Datagrid<T> where T : DatagridRows
    {
        [Parameter]
        public List<T>? Data { get; set; } = default!;

        private List<T>? _data;

        [Parameter]
        public List<DropdownColumn>? DropdownColumns { get; set; } = default!;

        [Parameter]
        public bool CanEditItem { get; set; } = false;

        [Parameter]
        public bool CanDeleteItem { get; set; } = false;

        [Parameter]
        public EventCallback<T> OnDelete { get; set; } = default!;

        [Parameter]
        public EventCallback<T> OnEdit { get; set; } = default!;

        [Inject]
        public IJSRuntime? JsRuntime { get; set; }

        List<Column> Columns = new List<Column>();

        List<string> ColumnsToIgnore = new List<string>() { "Category", "RowId", "Id", "CreatedOn", "CreatedBy" };

        protected override void OnInitialized()
        {
            _data = Data;
            Columns = CreateColumns(data: _data);
            ApplySorting();
        }

        protected override void OnParametersSet()
        {
            if (_data != Data)
            {
                _data = Data;
                ApplySorting();
            }
        }

        private List<Column> CreateColumns(List<T> data)
        {
            var columns = new List<Column>();

            if (data?.Count() > 0)
            {
                foreach (var prop in data[0].GetType().GetProperties())
                {
                    if (!ColumnsToIgnore.Contains(prop.Name))
                    {
                        if (DropdownColumns.Select(x => x.Name).ToList().Contains(prop.Name))
                        {
                            columns.Add(new Column
                            {
                                Name = prop.Name,
                                ColumnType = "Dropdown",
                                Dropdown = DropdownColumns.FirstOrDefault(x => x.Name == prop.Name)
                            });
                        }
                        else
                        {
                            columns.Add(new Column
                            {
                                Name = prop.Name,
                                ColumnType = prop.PropertyType.Name,
                            });
                        }
                    }
                }
            }
            return columns;
        }

        private void ToggleSort(Column column)
        {
            if (column != null)
            {
                switch (column.Sort)
                {
                    case SortDirection.Ascending:
                        column.Sort = SortDirection.Descending;
                        break;
                    case SortDirection.Descending:
                        column.Sort = SortDirection.None;
                        break;
                    case SortDirection.None:
                        column.Sort = SortDirection.Ascending;
                        break;
                    default:
                        break;

                }
                if (column.Sort == SortDirection.None)
                {
                    column.SortOrder = -1;
                }
                else
                {
                    column.SortOrder = Columns.Where(x => x.Name != column.Name).Max(x => x.SortOrder) + 1;
                }

                ApplySorting();
            }
        }

        private void ApplySorting()
        {
            int sortCounter = 0;

            foreach (var column in Columns.Where(x => x.SortOrder > -1).OrderBy(x => x.SortOrder))
            {
                if (column.Sort == SortDirection.Ascending)
                {
                    PropertyInfo prop = typeof(T).GetProperty(column.Name);
                    _data = (_data.AsQueryable().OrderBy(x => prop.GetValue(x, null))).ToList();
                }
                else if (column.Sort == SortDirection.Descending)
                {
                    PropertyInfo prop = typeof(T).GetProperty(column.Name);
                    _data = (_data.AsQueryable().OrderByDescending(x => prop.GetValue(x, null))).ToList();
                }

                column.SortOrder = sortCounter;
                sortCounter++;
            }
        }

        private async Task Edit(T item)
        {
            await OnEdit.InvokeAsync(item);
        }

        private static object GetPropValue(object src, string propName)
        {
            return src?.GetType()?.GetProperty(propName)?.GetValue(src, null);
        }

        //DELETE
        private async Task DeleteItem(T item)
        {
            if (_data.Contains(item))
            {
                bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
                if (confirmed)
                {
                    await OnDelete.InvokeAsync(item);
                }
            }
        }
    }
}