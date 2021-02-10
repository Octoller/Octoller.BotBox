using System.ComponentModel.DataAnnotations.Schema;

namespace Octoller.BotBox.Web.Models {

    public class Account {

        /// <summary>
        /// Id
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id {
            get; set;
        }

        /// <summary>
        /// Id на строне сервиса Vk
        /// </summary>
        public string VkId {
            get; set;
        }

        /// <summary>
        /// Полное имя на стороне сервиса Vk
        /// </summary>
        public string Name {
            get; set;
        }

        /// <summary>
        /// Ссылка на фотографию на стороне вк
        /// </summary>
        public string Photo {
            get; set;
        }

        /// <summary>
        /// Ключ доступа
        /// </summary>
        public string AccessToken {
            get; set;
        }

        /// <summary>
        /// Id пользователя
        /// </summary>
        [ForeignKey("User")]
        public string UserId {
            get; set;
        }

        public User User {
            get; set;
        }
    }
}
