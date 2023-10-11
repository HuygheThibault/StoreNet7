using Microsoft.EntityFrameworkCore;
using Store.Api.Models;

namespace Store.Api.Repositories
{
    public class OrderLineRepository : IOrderLineRepository
    {
        private readonly StoreContext context;
        private readonly ILogger<OrderLineRepository> logger;

        public OrderLineRepository(StoreContext context, ILogger<OrderLineRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<OrderLine>> GetAllOrderLines()
        {
            try
            {
                logger.LogInformation($"Getting all OrderLines");

                IQueryable<OrderLine> query = context.OrderLines.Include("Category");

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task<OrderLine> GetOrderLineById(Guid id)
        {
            try
            {
                logger.LogInformation($"Getting OrderLine: {id}");

                IQueryable<OrderLine> query = context.OrderLines.Include("Category");

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

        public void AddOrderLine(OrderLine orderLine)
        {
            try
            {
                logger.LogInformation($"Adding an orderLine {orderLine} to the context.");

                orderLine.Id = Guid.NewGuid();
                orderLine.CreatedOn = DateTime.Now;
                orderLine.ModifiedOn = DateTime.Now;

                Product product = context.Products.First(x => x.Id == orderLine.ProductId);
                product.QuantityInStock += orderLine.Quantity;

                context.OrderLines.Add(orderLine);
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

    public interface IOrderLineRepository
    {
        Task<List<OrderLine>> GetAllOrderLines();

        Task<OrderLine> GetOrderLineById(Guid id);

        void AddOrderLine(OrderLine orderLine);

        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
    }
}
