namespace Octoller.BotBox.Web.Models.Base {

    public class UserRelation : Auditable {
        
        /// <summary>
        /// Id пользователя
        /// </summary>
        public string UserId {
            get; set;
        }

        /// <summary>
        /// Пользователь
        /// </summary>
        public User User {
            get; set;
        }
    }
}
