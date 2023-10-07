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

                IQueryable<Order> query = context.Orders.Include("Supplier").Include("OrderLines");

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

                IQueryable<Order> query = context.Orders.Include("Supplier").Include("OrderLines");

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
            try
            {
                logger.LogInformation($"Adding an order: {order}.");

                order.Id = Guid.NewGuid();
                order.CreatedOn = DateTime.Now;
                order.ModifiedOn = DateTime.Now;

                context.Orders.Add(order);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
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
                        dbOrder.OrderLines.Remove(dbOrderline);
                    }

                    foreach (OrderLine dbOrderline in dbOrder.OrderLines.Where(x => order.OrderLines.Select(ol => ol.Id).Contains(x.Id))) // Updating order lines
                    {
                        logger.LogInformation($"Updating orderLine: {dbOrderline}.");

                        OrderLine newOrderLine = order.OrderLines.FirstOrDefault(x => x.Id == dbOrderline.Id);
                        dbOrderline.ProductId = newOrderLine.ProductId;
                        dbOrderline.Quantity = newOrderLine.Quantity;
                        dbOrderline.NetCost = newOrderLine.NetCost;
                        dbOrderline.VatCost = newOrderLine.VatCost;
                        dbOrderline.ModifiedOn = DateTime.Now;
                        dbOrderline.ModifiedBy = "Admin";
                    }

                    foreach (OrderLine orderLine in order.OrderLines.Where(x => x.Id == Guid.Empty)) // Add order lines
                    {
                        orderLine.ModifiedOn = DateTime.Now;
                        orderLine.CreatedOn = DateTime.Now;
                        orderLine.CreatedBy = "Admin";
                        orderLine.ModifiedBy = "Admin";
                        orderLine.OrderId = order.Id;

                        logger.LogInformation($"Adding orderLine: {orderLine}.");
                        dbOrder.OrderLines.Add(orderLine);
                    }

                    dbOrder.SupplierId = order.SupplierId;
                    dbOrder.TotalCost = order.TotalCost;
                    dbOrder.TotalVatCost = order.TotalVatCost;
                    dbOrder.IsPaid = order.IsPaid;
                    dbOrder.ModifiedBy = "Admin";
                    dbOrder.ModifiedOn = DateTime.Now;

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
