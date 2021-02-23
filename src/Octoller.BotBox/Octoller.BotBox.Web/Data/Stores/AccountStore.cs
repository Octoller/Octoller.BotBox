using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Data.Stores.Base;
using Octoller.BotBox.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Data.Stores
{
    public class AccountStore : BaseStore<ApplicationDbContext>
    {
        public AccountStore(ApplicationDbContext context) : base(context) 
        { 
        }

        private DbSet<Account> AccountsSet => Context.Set<Account>();
        public IQueryable<Account> Accounts => AccountsSet;

        public async Task<bool> CreateAsync(Account account, string initiator = null)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            await Context.AddAsync(account);
            await SaveChangeAsync(initiator);

            return true;
        }

        public async Task<bool> UpdateAsync(Account account, string initiator = null)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            Context.Attach(account);
            Context.Update(account);

            try
            {
                await SaveChangeAsync(initiator);
            }
            catch
            {
                ///TODO: передача ошибки выше по стеку
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(Account account)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            Context.Remove(account);

            try
            {
                await SaveChangeAsync();
            } 
            catch
            {
                ///TODO: передача ошибки выше по стеку
                return false;
            }

            return true;
        }

        public async Task<Account> GetByIdAsync(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await AccountsSet.FindAsync(new[] { id });
        }

        public async Task<Account> GetByUserIdAsync(string userId)
        {
            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await AccountsSet
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}
