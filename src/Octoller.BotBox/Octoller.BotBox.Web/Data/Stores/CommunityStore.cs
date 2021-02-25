using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Data.Models;
using Octoller.BotBox.Web.Data.Stores.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Data.Stores
{
    public class CommunityStore : BaseStore<ApplicationDbContext>
    {
        public CommunityStore(ApplicationDbContext context) : base(context) 
        {
        }

        private DbSet<Community> CommunitySet => Context.Set<Community>();
        public IQueryable<Community> Communities => CommunitySet;

        public async Task<bool> CreateAsync(Community community, string initiator = null)
        {
            if (community is null)
            {
                throw new ArgumentNullException(nameof(community));
            }

            await Context.AddAsync(community);
            await SaveChangeAsync(initiator);

            return true;
        }

        public async Task<bool> UpdateAsync(Community community, string initiator = null)
        {
            if (community is null)
            {
                throw new ArgumentNullException(nameof(community));
            }

            Context.Attach(community);
            Context.Update(community);

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

        public async Task<bool> DeleteGroupAsync(Community community)
        {
            if (community is null)
            {
                throw new ArgumentNullException(nameof(community));
            }

            Context.Remove(community);

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

        public async Task<Community> GetByIdAsync(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await CommunitySet.FindAsync(new[] { id });
        }

        public IQueryable<Community> GetByUserId(string userId)
        {
            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return CommunitySet.Where(a => a.UserId == userId);
        }
    }
}
