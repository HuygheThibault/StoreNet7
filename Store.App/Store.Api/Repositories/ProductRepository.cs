using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Store.Api.Models;
using Store.Shared.Modals;

namespace Store.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(StoreContext context, ILogger<ProductRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }

        public async Task<(IEnumerable<Product>, PaginationMetadata)> GetAllProducts(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation($"Getting all product");

                var collection = _context.Products as IQueryable<Product>;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    name = name.Trim();
                    collection = collection.Where(c => c.Title == name);
                }

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    collection = collection.Where(x => (x.Title != null && x.Title.Contains(searchQuery)));
                }

                var totalItemCount = await collection.CountAsync();

                var paginationMetadata = new PaginationMetadata(
                    totalItemCount, pageSize, pageNumber);

                var collectionToReturn = await collection.Include(x => x.Category)
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

        public async Task<Product> GetProductById(Guid id)
        {
            try
            {
                _logger.LogInformation($"Getting Product: {id}");

                IQueryable<Product> query = _context.Products.Include(x => x.Category);

                // Query It
                query = query.Where(c => c.Id == id);

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task<Product> GetProductByTitle(string title)
        {
            try
            {
                _logger.LogInformation($"Getting Product: {title}");

                IQueryable<Product> query = _context.Products.Include(x => x.Category);

                // Query It
                query = query.Where(c => c.Title == title);

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                throw;
            }
        }

        public async Task AddProduct(Product product)
        {
            try
            {
                _logger.LogInformation($"Adding an product: {product}.");
                product.Category = null;
                _context.Products.Add(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
            }
        }

        public void Delete(Product product)
        {
            try
            {
                _logger.LogInformation($"Removing product {product} from the context.");
                _context.Remove(product);
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

    public interface IProductRepository
    {
        Task<(IEnumerable<Product>, PaginationMetadata)> GetAllProducts(string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<Product> GetProductById(Guid id);

        Task<Product> GetProductByTitle(string title);

        Task AddProduct(Product product);

        void Delete(Product product);

        Task<bool> SaveChangesAsync();
    }
}
