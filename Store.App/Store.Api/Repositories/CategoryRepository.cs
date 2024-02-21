using Microsoft.EntityFrameworkCore;
using Store.Api.Models;

namespace Store.Api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreContext context;
        private readonly ILogger<CategoryRepository> logger;

        public CategoryRepository(StoreContext context, ILogger<CategoryRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            try
            {
                logger.LogInformation($"Getting all Categories");

                IQueryable<Category> query = context.Categories;

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task<Category> GetCategoryById(Guid id) 
        {
            try
            {
                logger.LogInformation($"Getting Category: {id}");

                IQueryable<Category> query = context.Categories;

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

        public async Task<Category> GetCategoryByTitle(string title)
        {
            try
            {
                logger.LogInformation($"Getting Category: {title}");

                IQueryable<Category> query = context.Categories;

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

    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategories();

        Task<Category> GetCategoryById(Guid id);

        Task<Category> GetCategoryByTitle(string title);

        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();
    }
}
