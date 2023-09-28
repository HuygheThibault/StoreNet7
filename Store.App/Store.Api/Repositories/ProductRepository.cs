using Microsoft.EntityFrameworkCore;
using Store.Api.Models;

namespace Store.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext context;
        private readonly ILogger<ProductRepository> logger;

        public ProductRepository(StoreContext context, ILogger<ProductRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                logger.LogInformation($"Getting all Products");

                IQueryable<Product> query = context.Products.Include("Category");

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task<Product> GetProductById(Guid id)
        {
            try
            {
                logger.LogInformation($"Getting Product: {id}");

                IQueryable<Product> query = context.Products.Include("Category");

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

        public async Task<Product> GetProductByTitle(string title)
        {
            try
            {
                logger.LogInformation($"Getting Product: {title}");

                IQueryable<Product> query = context.Products.Include("Category");

                // Query It
                query = query.Where(c => c.Title == title);

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

    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();

        Task<Product> GetProductById(Guid id);

        Task<Product> GetProductByTitle(string title);

        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
    }
}
