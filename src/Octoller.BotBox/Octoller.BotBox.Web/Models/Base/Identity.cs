using Octoller.BotBox.Web.Models.Abstraction;

namespace Octoller.BotBox.Web.Models.Base 
{
    public class Identity : IIdentity 
    {
        ///<inheritdoc />
        public string Id { get; set; }
    }
}
