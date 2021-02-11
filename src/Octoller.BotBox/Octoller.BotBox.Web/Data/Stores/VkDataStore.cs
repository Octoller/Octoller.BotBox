using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Data.Stores {

    public class VkDataStore {

        private readonly ApplicationDbContext context;

        private DbSet<Account> AccountsSet {
            get {
                return context.Set<Account>();
            }
        }

        private DbSet<Community> GroupsSet {
            get {
                return context.Set<Community>();
            }
        }

        /// <summary>
        /// Навигационное свойство
        /// </summary>
        public IQueryable<Account> Accounts {
            get {
                return AccountsSet;
            }
        }

        /// <summary>
        /// Навигационное свойство
        /// </summary>
        public IQueryable<Community> Groups {
            get {
                return GroupsSet;
            }
        }

        public VkDataStore(ApplicationDbContext context) {

            if (context is null) {
                throw new System.ArgumentNullException(nameof(context));
            }

            this.context = context;
        }

        public async Task<bool> CreateAccountAsync(Account account) {

            if (account is null)
                throw new System.ArgumentNullException(nameof(account));

            _ = await AccountsSet.AddAsync(account);
            _ = await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateGroupAsync(Community group) {

            if (group is null)
                throw new System.ArgumentNullException(nameof(group));

            _ = await GroupsSet.AddAsync(group);
            _ = await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAccountAsync(Account account) {

            if (account is null)
                throw new System.ArgumentNullException(nameof(account));

            AccountsSet.Update(account);
            _ = await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateGroupAsync(Community group) {

            if (group is null)
                throw new System.ArgumentNullException(nameof(group));

            GroupsSet.Update(group);
            _ = await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAccountAsync(Account account) {

            if (account is null)
                throw new System.ArgumentNullException(nameof(account));

            AccountsSet.Remove(account);
            _ = await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteGroupAsync(Community group) {

            if (group is null)
                throw new System.ArgumentNullException(nameof(group));

            GroupsSet.Remove(group);
            _ = await context.SaveChangesAsync();

            return true;
        }

        public async Task<Account> GetAccountByIdAsync(string id) {

            if (id is null)
                throw new System.ArgumentNullException(nameof(id));

            return await AccountsSet.FindAsync(new[] { id });
        }

        public async Task<Community> GetGroupByIdAsync(string id) {

            if (id is null)
                throw new System.ArgumentNullException(nameof(id));

            return await GroupsSet.FindAsync(new[] { id });
        }

        public async Task<Account> GetAccountByUserIdAsync(string userId) {

            if (userId is null)
                throw new System.ArgumentNullException(nameof(userId));

            return await AccountsSet
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public IQueryable<Community> GetGroupByUserId(string userId) {

            if (userId is null)
                throw new System.ArgumentNullException(nameof(userId));

            return GroupsSet.Where(a => a.UserId == userId);
        }
    }
}
