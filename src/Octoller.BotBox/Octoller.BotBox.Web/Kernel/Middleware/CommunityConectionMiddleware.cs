﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Octoller.BotBox.Web.Kernel.AuthenticationCommunity;
using System.Threading.Tasks;

namespace Octoller.BotBox.Web.Kernel.Middleware 
{
    public class CommunityConectionMiddleware 
    {
        private readonly RequestDelegate next;

        public CommunityConectionMiddleware(RequestDelegate next) 
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            string urlsection = context.Request.Path.Value.ToLower();

            HandlerAuthCommunity handler = context.RequestServices
                .GetRequiredService<HandlerAuthCommunity>();

            if (await handler.HandleRequstAsync()) 
            {
                return;
            }

            await next.Invoke(context);
        }
    }
}