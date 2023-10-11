using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Store.Api.Models;
using Store.Shared.Dto;

namespace Store.Api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreContext context;
        private readonly ILogger<OrderRepository> logger;

        public OrderRepository(StoreContext context, ILogger<OrderRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            try
            {
                logger.LogInformation($"Getting all Orders");

                IQueryable<Order> query = context.Orders.Include(x => x.Supplier).Include(x => x.OrderLines);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            try
            {
                logger.LogInformation($"Getting Order: {id}");

                IQueryable<Order> query = context.Orders.Include(x => x.Supplier).Include(x => x.OrderLines).ThenInclude(x => x.Product);

                // Query It
                query = query.Where(c => c.Id == id);

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
                throw;
            }
        }

        public void AddOrder(Order order)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {

                try
                {
                    order.Id = Guid.NewGuid();
                    logger.LogInformation($"Adding an order: {order}.");

                    context.Orders.Add(order);

                    foreach (OrderLine orderLine in order.OrderLines)
                    {
                        logger.LogInformation($"Adding an orderLine {orderLine} to the context.");
                        orderLine.OrderId = order.Id;
                        orderLine.Id = Guid.NewGuid();

                        Product product = context.Products.First(x => x.Id == orderLine.ProductId);
                        product.QuantityInStock += orderLine.Quantity;

                        context.OrderLines.Add(orderLine);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "error occured");
                    transaction.Rollback();
                }
            }
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    logger.LogInformation($"Updating order: {order.Id}.");

                    Order dbOrder = await GetOrderById(order.Id);

                    foreach (OrderLine dbOrderline in dbOrder.OrderLines.Where(x => !order.OrderLines.Select(ol => ol.Id).Contains(x.Id))) // Remove order lines
                    {
                        logger.LogInformation($"Removed orderLine: {dbOrderline.Id}.");

                        dbOrderline.Product.QuantityInStock -= dbOrderline.Quantity;
                        dbOrder.OrderLines.Remove(dbOrderline);
                    }

                    foreach (OrderLine dbOrderline in dbOrder.OrderLines.Where(x => order.OrderLines.Select(ol => ol.Id).Contains(x.Id))) // Updating order lines
                    {
                        logger.LogInformation($"Updating orderLine: {dbOrderline}.");

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
                        logger.LogInformation($"Adding orderLine: {orderLine}.");
                        orderLine.OrderId = order.Id;

                        Product product = context.Products.FirstOrDefault(x => x.Id == orderLine.ProductId);

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

                    context.Update(dbOrder);

                    await context.SaveChangesAsync();

                    transaction.Commit();

                    return dbOrder;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "error occured while updating order");
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Delete<T>(T entity) where T : class
        {
            try
            {
                logger.LogInformation($"Removing an object of type {entity.GetType()} to the context.");
                context.Remove(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                logger.LogInformation($"Attempitng to save the changes in the context");

                // Only return success if at least one row was changed
                return (await context.SaveChangesAsync()) > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
                throw;
            }
        }

    }

    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();

        Task<Order> GetOrderById(Guid id);

        Task<Order> UpdateOrder(Order order);

        void AddOrder(Order order);

        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
    }
}
