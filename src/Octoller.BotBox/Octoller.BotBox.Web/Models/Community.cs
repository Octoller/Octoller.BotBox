using System.ComponentModel.DataAnnotations.Schema;

namespace Octoller.BotBox.Web.Models {

    public class Community {
        
        /// <summary>
        /// Id
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id {
            get; set;
        }

        /// <summary>
        /// Id группы на стороне сервиса Vk
        /// </summary>
        public string VkId {
            get; set;
        }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Name {
            get; set;
        }

        /// <summary>
        /// Url заглавной фотографии
        /// </summary>
        public string Photo {
            get; set;
        }

        /// <summary>
        /// Указывает, является ли сообщество подключенным
        /// </summary>
        public bool Connected {
            get; set;
        }

        /// <summary>
        /// Подключенный шаблон бота
        /// </summary>
        public string TemplateBot {
            get; set;
        }

        /// <summary>
        /// Ключ доступа для упарвления через Api Vk
        /// </summary>
        public string AccessToken {
            get; set;
        }

        /// <summary>
        /// id пользовтаеля
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
