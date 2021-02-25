using Microsoft.EntityFrameworkCore;
using Octoller.BotBox.Web.Data.Base;
using Octoller.BotBox.Web.Data.Models;

namespace Octoller.BotBox.Web.Data 
{
    public class ApplicationDbContext : DbContextBase<User> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { }
    }
}
