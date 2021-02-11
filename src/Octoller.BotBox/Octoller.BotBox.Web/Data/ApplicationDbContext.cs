using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Models;

namespace Octoller.BotBox.Web.Data {

    public class ApplicationDbContext : IdentityDbContext<User> {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder builder) {

            builder.ApplyConfigurationsFromAssembly(typeof(Startup).Assembly);

            base.OnModelCreating(builder);


        }
    }
}
