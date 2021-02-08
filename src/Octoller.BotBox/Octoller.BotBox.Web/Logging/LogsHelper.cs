using Microsoft.Extensions.Logging;

namespace Octoller.BotBox.Web.Logging {

    public static class LogsHelper {

        public static readonly EventId ConectedVkId = new EventId(
            id: LogEventsId.ConectedVk,
            name: "ConectedVk");

    }
}
