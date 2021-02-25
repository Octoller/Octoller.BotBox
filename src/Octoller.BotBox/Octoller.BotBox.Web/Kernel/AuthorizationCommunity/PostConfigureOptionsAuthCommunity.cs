using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    public class PostConfigureOptionsAuthCommunity : IPostConfigureOptions<OptionsAuthCommunity> 
    {
        private readonly IDataProtectionProvider protectionProvider;

        public PostConfigureOptionsAuthCommunity(IDataProtectionProvider protectionProvider) 
        {
            this.protectionProvider = protectionProvider;
        }

        ///<inheritdoc />
        public void PostConfigure(string name, OptionsAuthCommunity options) 
        {
            if (options.Backchannel is null) 
            {
                options.Backchannel = new HttpClient(new HttpClientHandler());
                options.Backchannel.Timeout = TimeSpan.FromSeconds(60);
                options.Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; //10 MB
            }

            if (options.StateDataFormat is null) 
            {
                var protector = protectionProvider.CreateProtector(
                    typeof(OptionsAuthCommunity).FullName, name, "v1");

                options.StateDataFormat = new DataProtectorAuthComunnity(DataSerializerAuthCommunity.Default, protector);
            }
        }
    }
}
