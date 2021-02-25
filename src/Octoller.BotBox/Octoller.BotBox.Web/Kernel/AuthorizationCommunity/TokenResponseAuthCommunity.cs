using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    public class TokenResponseAuthCommunity 
    {
        /// <summary>
        /// Получает или задает ответный JSON документ.
        /// </summary>
        public JObject Response { get; set; }

        public List<AuthToken> Tokens { get; } 

        public Exception Error { get; set; }

        private TokenResponseAuthCommunity(JObject response) 
        {
            Response = response;
            JToken groups = response.GetValue("groups");
            Tokens = new List<AuthToken>(); 

            foreach (var item in groups) 
            {
                Tokens.Add(new AuthToken 
                {
                    Community = item.Value<string>("group_id"),
                    Value = item.Value<string>("access_token")
                });
            };
        }

        private TokenResponseAuthCommunity(Exception exception) 
        {
            Error = exception;
        }

        public static TokenResponseAuthCommunity Success(JObject response) 
        {
            return new TokenResponseAuthCommunity(response);
        }

        public static TokenResponseAuthCommunity Failed(Exception error) 
        {
            return new TokenResponseAuthCommunity(error);
        }
    }
}
