using Octoller.BotBox.Web.Models.Abstraction;
using System;

namespace Octoller.BotBox.Web.Models
{
    public class Account : IIdentity, IAuditable
    {
        ///<inheritdoc />
        public string Id { get; set; }
        
        /// <summary>
        /// Id пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Id группы на стороне сервиса Vk
        /// </summary>
        public string VkId { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url заглавной фотографии
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// Ключ доступа
        /// </summary>
        public string AccessToken { get; set; }

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
