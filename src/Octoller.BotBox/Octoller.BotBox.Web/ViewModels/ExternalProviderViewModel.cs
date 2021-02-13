using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace Octoller.BotBox.Web.ViewModels {

    public class ExternalProviderViewModel {
        
        //Коллекция имен на случай, если будет несколько провайдеров аутентификации

        public IEnumerable<AuthenticationScheme> Providers {
            get; set;
        }
    }
}
