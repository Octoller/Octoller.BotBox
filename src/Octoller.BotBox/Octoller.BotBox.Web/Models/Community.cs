using Octoller.BotBox.Web.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Octoller.BotBox.Web.Models {

    public class Community : VkDataEntity {

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
    }
}
