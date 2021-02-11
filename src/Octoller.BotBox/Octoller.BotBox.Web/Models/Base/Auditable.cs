using Octoller.BotBox.Web.Models.Abstraction;
using System;

namespace Octoller.BotBox.Web.Models.Base {

    public class Auditable : Identity, IAuditable {

        ///<inheritdoc />
        public DateTime CreatedAt {
            get; set;
        }

        ///<inheritdoc />
        public string CreateBy {
            get; set;
        }

        ///<inheritdoc />
        public DateTime? UpdateAt {
            get; set;
        }

        ///<inheritdoc />
        public string UpdateBy {
            get; set;
        }
    }
}
