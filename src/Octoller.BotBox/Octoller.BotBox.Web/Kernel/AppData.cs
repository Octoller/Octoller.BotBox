using System.Collections.Generic;

namespace Octoller.BotBox.Web.Kernel {

    public static class AppData {

        public static class ExternalAuthProvider {

            public const string VkProviderName = "VK";

            public static IEnumerable<string> Providers {
                get {
                    yield return VkProviderName;
                }
            }
        }

        public static class RolesData {

            public const string AdministratorRoleName = "Administrator";
            public const string UserRoleName = "User";

            public static IEnumerable<string> Roles {
                get {
                    yield return AdministratorRoleName;
                    yield return UserRoleName;
                }
            }
        }
    }
}
