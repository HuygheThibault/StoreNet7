using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Store.Api.Models;
using Store.Shared.Modals;

namespace Store.Api.Repositories
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private readonly StoreContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(StoreContext context, ILogger<OrderRepository> logger) : base(context, logger)
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

                if (!string.IsNullOrWhiteSpace(name))
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

        public async Task<Order> AddOrder(Order order)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation($"Adding an order: {order}.");

                    // Check if the order already exists in the database
                    if (await GetOrderById(order.Id) != null)
                    {
                        _logger.LogError($"Order with ID {order.Id} already exists.");
                        return null;
                    }

                    // Retrieve the supplier from the database
                    var supplier = await _context.Suppliers.FindAsync(order.SupplierId);
                    if (supplier == null)
                    {
                        _logger.LogError($"Supplier with ID {order.SupplierId} not found.");
                        return null;
                    }
                    order.Supplier = supplier;

                    // Add the order to the context (but don't save the changes yet)
                    _context.Orders.Add(order);

                    foreach (OrderLine orderLine in order.OrderLines)
                    {
                        _logger.LogInformation($"Adding an orderLine {orderLine} to the context.");

                        // Retrieve the product from the database
                        var product = await _context.Products.FindAsync(orderLine.ProductId);
                        if (product == null)
                        {
                            _logger.LogError($"Product with ID {orderLine.ProductId} not found.");
                            continue;
                        }

                        // Update the quantity in stock
                        product.QuantityInStock += orderLine.Quantity;

                        orderLine.ProductId = product.Id; // Set the product ID
                        orderLine.OrderId = order.Id;

                        // Add the order line to the context
                        _context.OrderLines.Add(orderLine);
                    }

                    // Calculate the total cost of the order
                    order.Cost = (decimal)order.OrderLines.Sum(x => x.Cost);

                    // Save changes and commit the transaction
                    if (await SaveChangesAsync())
                    {
                        transaction.Commit();
                        return order;
                    }
                    else
                    {
                        _logger.LogError("No rows were affected by the add operation.");
                        transaction.Rollback();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while adding order");
                    transaction.Rollback();
                    throw;
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

                    Order dbOrder = await _context.Orders.Include(x => x.OrderLines).ThenInclude(x => x.Product).SingleOrDefaultAsync(c => c.Id == order.Id);

                    if (dbOrder == null)
                    {
                        _logger.LogError($"Order with ID {order.Id} not found.");
                        return null;
                    }

                    // Update parent
                    _context.Entry(dbOrder).CurrentValues.SetValues(order);

                    // Delete children
                    var orderLineIdsToDelete = dbOrder.OrderLines.Where(x => !order.OrderLines.Any(ol => ol.Id == x.Id)).Select(x => x.Id).ToList();
                    var orderLinesToDelete = await _context.OrderLines.Where(x => orderLineIdsToDelete.Contains(x.Id)).ToListAsync();
                    _context.OrderLines.RemoveRange(orderLinesToDelete);

                    // Update and Insert children
                    foreach (OrderLine orderLine in order.OrderLines)
                    {
                        var existingChild = dbOrder.OrderLines.FirstOrDefault(c => c.Id == orderLine.Id);

                        if (existingChild != null)
                        {
                            // Update child
                            _logger.LogInformation($"Updating orderLine: {orderLine}.");
                            _context.Entry(existingChild).CurrentValues.SetValues(orderLine);
                            existingChild.Product.QuantityInStock += (orderLine.Quantity - existingChild.Quantity);
                        }
                        else
                        {
                            // Insert child
                            _logger.LogInformation($"Adding orderLine: {orderLine}.");
                            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == orderLine.ProductId);
                            if (product == null)
                            {
                                _logger.LogError($"Product with ID {orderLine.ProductId} not found.");
                                continue;
                            }
                            product.QuantityInStock += orderLine.Quantity;
                            dbOrder.OrderLines.Add(orderLine);
                            _context.Entry(orderLine).State = EntityState.Added;  // Explicitly set the state to Added
                        }
                    }

                    if (await SaveChangesAsync())
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        _logger.LogError("No rows were affected by the update operation.");
                        transaction.Rollback();
                    }

                    return dbOrder;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating order");
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task Delete(Order order)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation($"Removing order {order.Id} from the context.");

                    // Load the order with its order lines from the database
                    var dbOrder = await _context.Orders.Include(o => o.OrderLines).SingleOrDefaultAsync(o => o.Id == order.Id);

                    if (dbOrder == null)
                    {
                        _logger.LogError($"Order with ID {order.Id} not found.");
                        return;
                    }

                    // Decrease the quantity in stock for each product
                    foreach (OrderLine orderLine in dbOrder.OrderLines)
                    {
                        var product = await _context.Products.FindAsync(orderLine.ProductId);
                        if (product != null)
                        {
                            product.QuantityInStock -= orderLine.Quantity;
                        }
                    }

                    // Remove the order, which will also remove the order lines because of the cascade delete
                    _context.Orders.Remove(dbOrder);

                    // Save changes and commit the transaction
                    if (await SaveChangesAsync())
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        _logger.LogError("No rows were affected by the delete operation.");
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while deleting order");
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public interface IOrderRepository
    {
        Task<(IEnumerable<Order>, PaginationMetadata)> GetAllOrders(string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<Order> GetOrderById(Guid id);

        Task<Order> AddOrder(Order order);

        Task<Order> UpdateOrder(Order order);

        Task Delete(Order order);
    }
}
