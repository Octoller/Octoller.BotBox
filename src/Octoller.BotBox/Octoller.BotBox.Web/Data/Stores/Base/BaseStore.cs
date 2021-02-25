using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Data.Models.Abstraction;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Data.Stores.Base
{
    public abstract class BaseStore<TContext> where TContext : DbContext
    {
        protected TContext Context { get; private set; }

        public BaseStore(TContext context)
        {
            this.Context = context;
        }

        protected async Task SaveChangeAsync(string initiator = null)
        {
            DateTime date = DateTime.UtcNow;

            if (string.IsNullOrEmpty(initiator))
            {
                initiator = "System";
            }

            var addedEntities = Context.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added);

            foreach (var entry in addedEntities)
            {
                if (entry.Entity is not IAuditable)
                {
                   await Task.CompletedTask;
                }

                var createdBy = entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue;
                var updatedBy = entry.Property(nameof(IAuditable.UpdatedBy)).CurrentValue;
                var createdAt = entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue;
                var updatedAt = entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue;

                if (string.IsNullOrEmpty(createdBy?.ToString()))
                {
                    entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = initiator;
                }

                if (string.IsNullOrEmpty(updatedBy?.ToString()))
                {
                    entry.Property(nameof(IAuditable.UpdatedBy)).CurrentValue = initiator;
                }

                if (DateTime.Parse(createdAt?.ToString()).Year < 1970)
                {
                    entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = date;
                }

                if (updatedAt != null && DateTime.Parse(updatedAt?.ToString()).Year < 1970)
                {
                    entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = date;
                } else
                {
                    entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = date;
                }
            }

            var modifiedEntities = Context.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified);

            foreach (var entry in modifiedEntities)
            {
                if (entry.Entity is IAuditable)
                {
                    entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                    entry.Property(nameof(IAuditable.UpdatedBy)).CurrentValue = initiator;
                }
            }

            _ = await Context.SaveChangesAsync();
        }
    }
}
