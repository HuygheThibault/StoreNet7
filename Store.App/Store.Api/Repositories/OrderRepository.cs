using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Store.Api.Models;
using Store.Shared.Modals;

namespace Store.Api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(StoreContext context, ILogger<OrderRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(IEnumerable<Order>, PaginationMetadata)> GetAllOrders(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation($"Getting all Orders");

                var collection = _context.Orders as IQueryable<Order>;

                if(!string.IsNullOrWhiteSpace(name))
                {
                    name = name.Trim();
                    collection = collection.Where(c => c.FileName == name);
                }

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    collection = collection.Where(x => (x.FileName != null && x.FileName.Contains(searchQuery)) || (x.Supplier != null && x.Supplier.Name.Contains(searchQuery)));
                }

                var totalItemCount = await collection.CountAsync();

                var paginationMetadata = new PaginationMetadata(
                    totalItemCount, pageSize, pageNumber);

                var collectionToReturn = await collection.Include(x => x.OrderLines)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

                return (collectionToReturn, paginationMetadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            try
            {
                _logger.LogInformation($"Getting Order: {id}");

                IQueryable<Order> query = _context.Orders.Include(x => x.Supplier).Include(x => x.OrderLines).ThenInclude(x => x.Product);

                return await query.FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task AddOrder(Order order)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation($"Adding an order: {order}.");

                    if(await GetOrderById(order.Id) == null)
                    {
                        _context.Orders.Add(order);

                        foreach (OrderLine orderLine in order.OrderLines)
                        {
                            _logger.LogInformation($"Adding an orderLine {orderLine} to the context.");
                         
                            orderLine.OrderId = order.Id;
                            _context.OrderLines.Add(orderLine);

                            Product product = _context.Products.First(x => x.Id == orderLine.ProductId);
                            product.QuantityInStock += orderLine.Quantity;
                        }
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "error occured");
                    transaction.Rollback();
                }
            }
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation($"Updating order: {order.Id}.");

                    Order dbOrder = await GetOrderById(order.Id);

                    foreach (OrderLine dbOrderline in dbOrder.OrderLines.Where(x => !order.OrderLines.Select(ol => ol.Id).Contains(x.Id))) // Remove order lines
                    {
                        _logger.LogInformation($"Removed orderLine: {dbOrderline.Id}.");

                        dbOrderline.Product.QuantityInStock -= dbOrderline.Quantity;
                        dbOrder.OrderLines.Remove(dbOrderline);
                    }

                    foreach (OrderLine dbOrderline in dbOrder.OrderLines.Where(x => order.OrderLines.Select(ol => ol.Id).Contains(x.Id))) // Updating order lines
                    {
                        _logger.LogInformation($"Updating orderLine: {dbOrderline}.");

                        OrderLine newOrderLine = order.OrderLines.FirstOrDefault(x => x.Id == dbOrderline.Id);

                        if (dbOrderline.Quantity != newOrderLine.Quantity)
                        {
                            dbOrderline.Product.QuantityInStock -= dbOrderline.Quantity;
                            dbOrderline.Product.QuantityInStock += newOrderLine.Quantity;
                        }

                        dbOrderline.ProductId = newOrderLine.ProductId;
                        dbOrderline.Quantity = newOrderLine.Quantity;
                        dbOrderline.CostPerItem = newOrderLine.CostPerItem;
                        dbOrderline.Cost = newOrderLine.Cost;
                    }

                    foreach (OrderLine orderLine in order.OrderLines.Where(x => x.Id == Guid.Empty)) // Add order lines
                    {
                        _logger.LogInformation($"Adding orderLine: {orderLine}.");
                        orderLine.OrderId = order.Id;

                        Product product = _context.Products.FirstOrDefault(x => x.Id == orderLine.ProductId);

                        if(product != null)
                        {
                            orderLine.Product = product;
                            orderLine.Product.QuantityInStock += orderLine.Quantity;
                        }

                        dbOrder.OrderLines.Add(orderLine);
                    }

                    dbOrder.SupplierId = order.SupplierId;
                    dbOrder.Cost = order.Cost;
                    dbOrder.Comments = order.Comments;
                    dbOrder.IsPaid = order.IsPaid;

                    _context.Update(dbOrder);

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return dbOrder;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "error occured while updating order");
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Delete(Order order)
        {
            try
            {
                _logger.LogInformation($"Removing order {order} from the context.");

                foreach(OrderLine orderLine in order.OrderLines)
                {
                    Product product = _context.Products.First(x => x.Id == orderLine.ProductId);
                    if(product != null )
                    {
                        product.QuantityInStock -= orderLine.Quantity;
                    }

                    _context.Remove(orderLine);
                }

                _context.Remove(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                _logger.LogInformation($"Attempitng to save the changes in the context");

                // Only return success if at least one row was changed
                return (await _context.SaveChangesAsync()) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                throw;
            }
        }

    }

    public interface IOrderRepository
    {
        Task<(IEnumerable<Order>, PaginationMetadata)> GetAllOrders(string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<Order> GetOrderById(Guid id);

        Task<Order> UpdateOrder(Order order);

        Task AddOrder(Order order);

        void Delete(Order order);

        Task<bool> SaveChangesAsync();
    }
}
