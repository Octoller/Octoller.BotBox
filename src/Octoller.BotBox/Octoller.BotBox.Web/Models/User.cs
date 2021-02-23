using Microsoft.AspNetCore.Identity;
using Octoller.BotBox.Web.Models.Abstraction;
using System;
using System.Collections.Generic;

namespace Octoller.BotBox.Web.Models 
{
    public class User : IdentityUser, IIdentity, IAuditable
    {
        /// <summary>
        /// Данные о аккаунте пользователя на стороне Vk
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Данные о сообществе на стороне Vk
        /// </summary>
        public IEnumerable<Community> Communities { get; set; }

        ///<inheritdoc />
        public DateTime CreatedAt { get; set; }

        ///<inheritdoc />
        public string CreatedBy { get; set; }

        ///<inheritdoc />
        public DateTime UpdatedAt { get; set; }

        ///<inheritdoc />
        public string UpdatedBy { get; set; }
    }
}
