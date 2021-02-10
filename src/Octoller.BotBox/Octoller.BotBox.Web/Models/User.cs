using Microsoft.AspNetCore.Identity;

namespace Octoller.BotBox.Web.Models {

    public class User : IdentityUser {

        public Account Account {
            get; set;
        }

        public Community Community {
            get; set;
        }
    }
}
