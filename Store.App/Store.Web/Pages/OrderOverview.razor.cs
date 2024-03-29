﻿using Microsoft.AspNetCore.Components;
using Store.Shared.Dto;
using Store.Shared.Enums;
using Store.Shared.Modals;
using Store.Web.Helpers.Modals;
using Store.Web.Models;
using Store.Web.Services;
using System;
using System.Reflection;
using static Store.Web.Models.Noticiation;

namespace Store.Web.Pages
{
    public partial class OrderOverview
    {
        [Inject]
        public NotificationService NotificationService { get; set; }

        [Inject]
        public IOrderService OrderService { get; set; }

        [Inject]
        public ISupplierService SupplierService { get; set; }

        [Inject]
        public IProductService ProductService { get; set; }

        public OrderDto? EditedOrder;

        public OrderDto? NewOrder;

        private OrderDto? _expandedOrder;

        private Noticiation? _noticiation;

        private List<OrderDto> _data;

        private List<SupplierDto> _suppliers;

        private List<ProductDto> _products;

        List<Column> Columns = new List<Column>();

        List<Column> SubColumns = new List<Column>();

        List<string> ColumnsToIgnore = new List<string>() { "OrderLines", "Supplier", "RowId", "Id", "CreatedOn", "CreatedBy" };

        PaginationMetadata Pagination = new PaginationMetadata();

        protected override async Task OnInitializedAsync()
        {
            await RefreshData();
        }

        private async Task GetGridData()
        {
            Tuple<IEnumerable<OrderDto>, PaginationMetadata> gridData;

            if (Pagination != null)
            {
                gridData = await OrderService.GetAllOrders(pageNumber: Pagination.CurrentPage, pageSize: Pagination.PageSize);
            }
            else
            {
                gridData = await OrderService.GetAllOrders();
            }

            if (gridData != null)
            {
                _data = gridData?.Item1.ToList();
                Pagination = gridData?.Item2;
            }

        }

        private async Task RefreshData()
        {
            await GetGridData();
            _suppliers = (await SupplierService.GetAllSuppliers()).ToList();
            _products = (await ProductService.GetAllProducts()).ToList();
            CreateColumns();
            ApplySorting();
            await InvokeAsync(StateHasChanged);
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
                    Name = "Cost",
                    ColumnType = "decimal",
                },
                new Column
                {
                    Name = "Comments",
                    ColumnType = "string",
                },
                new Column
                {
                    Name = "IsPaid",
                    ColumnType = "Bool",
                },
                new Column
                {
                    Name = "ExpirationDate",
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
                    Name = "CostPerItem",
                    ColumnType = "decimal",
                },
                new Column
                {
                    Name = "Cost",
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
            NewOrder = new OrderDto();
        }

        private void Edit(OrderDto item)
        {
            EditedOrder = item;
        }

        private async Task DeleteItem(OrderDto item)
        {
            bool isDeleted = await OrderService.DeleteOrder(item.Id);
            if (isDeleted)
            {
                NotificationService.ShowNotification(new Noticiation()
                {
                    Name = $"{item.FileName} deleted",
                    Level = NoticiationLevel.Success
                });
                await RefreshData();
            }
            else
            {
                NotificationService.ShowNotification(new Noticiation()
                {
                    Name = $"Failed to delete {item.FileName}",
                    Level = NoticiationLevel.Danger
                });
            }
        }

        public async Task OrderDialogResultReceived(OrderDto order)
        {
            EditedOrder = null;
            if (order != null)
            {
                await GetGridData();
            }
        }

        public async Task OrderWizardResultReceived(OrderDto order)
        {
            NewOrder = null;
            if (order != null)
            {
                await RefreshData();
            }
        }

        private static object GetPropValue(object src, string propName)
        {
            return src?.GetType()?.GetProperty(propName)?.GetValue(src, null);
        }
    }
}
