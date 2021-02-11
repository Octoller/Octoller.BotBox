namespace Octoller.BotBox.Web.Models.Base {

    public class UserRelation : Auditable {
        
        public string UserId {
            get; set;
        }

        public User User {
            get; set;
        }
    }
}
