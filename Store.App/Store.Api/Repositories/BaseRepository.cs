using Microsoft.EntityFrameworkCore;
using Store.Api.Models;
using Store.Shared.Modals;

namespace Store.Api.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly StoreContext _context;
        protected readonly ILogger _logger;

        protected BaseRepository(StoreContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                _logger.LogInformation($"Attempting to save the changes in the context");

                // Set modified by and modified on properties
                var modifiedEntities = _context.ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

                foreach (var entry in modifiedEntities)
                {
                    if (entry.Entity is BaseEntity entity)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedBy = "YourCreatedByValue";
                            entity.CreatedOn = DateTime.Now;
                            entity.ModifiedBy = "YourModifiedByValue";
                            entity.ModifiedOn = DateTime.Now;
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.ModifiedBy = "YourModifiedByValue";
                            entity.ModifiedOn = DateTime.Now;
                        }
                    }
                }

                // Only return success if at least one row was changed
                return (await _context.SaveChangesAsync()) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occurred");
                throw;
            }
        }
    }
}
