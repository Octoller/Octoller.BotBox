using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Models;

namespace Octoller.BotBox.Web.Data {

    public class ApplicationDbContext : IdentityDbContext<User> {
        
        public DbSet<Account> Accounts {
            get; set;
        }

        public DbSet<Community> Communities {
            get; set;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder builder) {
            
            base.OnModelCreating(builder);


        }
    }
}
