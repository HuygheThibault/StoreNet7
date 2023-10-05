using Microsoft.EntityFrameworkCore;
using Store.Api.Models;

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

                IQueryable<Order> query = context.Orders.Include("Supplier");

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

                IQueryable<Order> query = context.Orders.Include("Supplier");

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
                order.Id = Guid.NewGuid();

                logger.LogInformation($"Adding an order: {order}.");
                foreach(OrderLine orderLine in order.OrderLines)
                {
                    orderLine.Id = Guid.NewGuid();
                    orderLine.OrderId = order.Id;
                    orderLine.CreatedOn = DateTime.Now;
                    orderLine.ModifiedOn = DateTime.Now;

                    context.OrderLines.Add(orderLine);
                }

                order.CreatedOn = DateTime.Now;
                order.ModifiedOn = DateTime.Now;

                context.Orders.Add(order);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
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

        void AddOrder(Order order);

        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
    }
}
