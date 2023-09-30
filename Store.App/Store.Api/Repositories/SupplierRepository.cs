using Microsoft.EntityFrameworkCore;
using Store.Api.Models;

namespace Store.Api.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly StoreContext context;
        private readonly ILogger<SupplierRepository> logger;

        public SupplierRepository(StoreContext context, ILogger<SupplierRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<Supplier>> GetAllSuppliers()
        {
            try
            {
                logger.LogInformation($"Getting all Suppliers");

                IQueryable<Supplier> query = context.Suppliers;

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task<Supplier> GetSupplierById(Guid id)
        {
            try
            {
                logger.LogInformation($"Getting Supplier: {id}");

                IQueryable<Supplier> query = context.Suppliers;

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

        public void Add<T>(T entity) where T : class
        {
            try
            {
                logger.LogInformation($"Adding an object of type {entity.GetType()} to the context.");
                context.Add(entity);
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

    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllSuppliers();

        Task<Supplier> GetSupplierById(Guid id);

        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
    }
}
