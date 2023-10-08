using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Shared.Enums;
using Store.Web.Helpers.Modals;
using Store.Web.Models;
using Store.Web.Services;
using System.Reflection;

namespace Store.Web.Pages
{
    public partial class OrderOverview
    {
        [Inject]
        public IOrderService OrderService { get; set; }

        [Inject]
        public ISupplierService SupplierService { get; set; }

        [Inject]
        public IProductService ProductService { get; set; }

        private OrderDto? _order;

        private OrderDto? _expandedOrder;

        private Noticiation? _noticiation;

        private List<OrderDto> _data;

        private List<SupplierDto> _suppliers;

        private List<ProductDto> _products;

        List<Column> Columns = new List<Column>();

        List<Column> SubColumns = new List<Column>();

        List<string> ColumnsToIgnore = new List<string>() { "OrderLines", "Supplier", "RowId", "Id", "CreatedOn", "CreatedBy" };

        protected override async Task OnInitializedAsync()
        {
            _data = (await OrderService.GetAllOrders()).ToList();
            _suppliers = (await SupplierService.GetAllSuppliers()).ToList();
            _products = (await ProductService.GetAllProducts()).ToList();
            CreateColumns();
            ApplySorting();
        }

        private void CreateColumns()
        {
            Columns = new List<Column>()
            {
                new Column()
                {
                    Name = "SupplierId",
                    ColumnType = "Dropdown",
                    Dropdown = new DropdownColumn()
                    {
                        Name = "SupplierId",
                        Values = _suppliers.Select(x => new DropdownOption()
                        {
                            Value = x.Name,
                            Id = x.Id,
                        }).ToList()
                    }
                },
                new Column
                {
                    Name = "FileName",
                    ColumnType = "string",
                },
                new Column
                {
                    Name = "TotalCost",
                    ColumnType = "decimal",
                },
                new Column
                {
                    Name = "TotalVatCost",
                    ColumnType = "decimal",
                },
                new Column
                {
                    Name = "IsPaid",
                    ColumnType = "Bool",
                },
                new Column
                {
                    Name = "CreatedOn",
                    ColumnType = "DateTime",
                },
                new Column()
                {
                    Name = "actions",
                    Sort = SortDirection.None
                }
            };

            SubColumns = new List<Column>()
            {
                new Column()
                {
                    Name = "ProductId",
                    ColumnType = "Dropdown",
                    Dropdown = new DropdownColumn()
                    {
                        Name = "ProductId",
                        Values = _products.Select(x => new DropdownOption()
                        {
                            Value = x.Title,
                            Id = x.Id,
                        }).ToList()
                    }
                },
                new Column
                {
                    Name = "Quantity",
                    ColumnType = "int32",
                },
                new Column
                {
                    Name = "NetCost",
                    ColumnType = "decimal",
                },
                new Column
                {
                    Name = "VatCost",
                    ColumnType = "decimal",
                }
            };
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
                    PropertyInfo prop = typeof(OrderDto).GetProperty(column.Name);
                    _data = (_data.AsQueryable().OrderBy(x => prop.GetValue(x, null))).ToList();
                }
                else if (column.Sort == SortDirection.Descending)
                {
                    PropertyInfo prop = typeof(OrderDto).GetProperty(column.Name);
                    _data = (_data.AsQueryable().OrderByDescending(x => prop.GetValue(x, null))).ToList();
                }

                column.SortOrder = sortCounter;
                sortCounter++;
            }
        }

        private void ExpandRow(OrderDto item)
        {
            if (_expandedOrder == item)
            {
                _expandedOrder = null;
            }
            else
            {
                _expandedOrder = item;
            }
        }

        private void AddOrder()
        {
            _order = new OrderDto();
        }

        private void Edit(OrderDto item)
        {
            _order = item;
        }

        private async Task DeleteItem(OrderDto item)
        {
            await OrderService.DeleteOrder(item.Id);
        }

        private async Task ResultReceived(Noticiation noticiation)
        {
            _order = null;
            _noticiation = noticiation;
            _data = (await OrderService.GetAllOrders()).ToList();
            _noticiation = null;
        }

        private static object GetPropValue(object src, string propName)
        {
            return src?.GetType()?.GetProperty(propName)?.GetValue(src, null);
        }
    }
}
