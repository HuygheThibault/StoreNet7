using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Store.Shared.Modals;

namespace Store.Api.Models
{
    public partial class StoreContext
    {
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).ModifiedOn = DateTime.Now;
                ((BaseEntity)entityEntry.Entity).ModifiedBy = "Ovveride";

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedOn = DateTime.Now;
                    ((BaseEntity)entityEntry.Entity).CreatedBy = "Ovveride";
                }
            }

            return base.SaveChanges();
        }
    }
}
