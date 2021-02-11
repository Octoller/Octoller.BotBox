using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Octoller.BotBox.Web.Models {

    public class User : IdentityUser {

        /// <summary>
        /// Данные о аккаунте пользователя на стороне Vk
        /// </summary>
        public Account Account {
            get; set;
        }

        /// <summary>
        /// Данные о сообществе на стороне Vk
        /// </summary>
        public IEnumerable<Community> Communities {
            get; set;
        }
    }
}
