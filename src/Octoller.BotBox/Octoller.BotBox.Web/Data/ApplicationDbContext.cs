using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Models;
using System;

namespace Octoller.BotBox.Web.Data {

    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid> {

        
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

        }

    }
}
