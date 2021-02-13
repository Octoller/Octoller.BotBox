﻿using Microsoft.AspNetCore.Http;

namespace Octoller.BotBox.Web.ViewModels {

    public class ProfileViewModel {

        public bool IsAccountConnected {
            get; set;
        }

        public string Name {
            get; set;
        }

        public byte[] Avatar {
            get; set;
        }

        public int CountTemplate {
            get; set;
        }

        public int CommunityConnectedCount {
            get; set;
        }
    }
}
