using Octoller.BotBox.Web.Kernel.Extension;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Linq;
using System.Text;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// Обработчик аутентификации сообщества ВК
    /// </summary>
    public class HandlerAuthCommunity 
    {
        private readonly HttpContext context;
        private readonly OptionsAuthCommunity options;

        public HandlerAuthCommunity(
            IOptions<OptionsAuthCommunity> options,
            IHttpContextAccessor accessor) 
        {
            this.options = options.Value;
            this.context = accessor.HttpContext;
            this.Events = new AuthCommunityEvents();
        }

        private HttpRequest Request => context.Request;
        private HttpResponse Response => context.Response;

        private AuthCommunityEvents Events { get; set; }

        public Task<bool> ShouldHandleRequestAsync()
            => Task.FromResult(options.CallbackPath == Request.Path);

        /// <summary>
        /// Обрабаотывает текущий запрос аутентификации сообщества
        /// </summary>
        /// <returns><see langword="true" /> если операция обработана успешно, иначе <see langword="false" />.</returns>
        public async Task<bool> HandleRequstAsync() 
        {
            if (!await ShouldHandleRequestAsync()) 
            {
                return false;
            }

            Exception exception = null;
            PropertiesAuthCommunity properties = null;
            TicketAuthCommunity tiket = null;

            try
            {
                var authResult = await HandleAuthenticationAsync();
                if (authResult is null)
                {
                    exception = new InvalidOperationException("Ошибка состояния, перенаправление не возможно");
                }
                else if (authResult.Handlered)
                {
                    return true;
                } 
                else if (authResult.Skippeded)
                {
                    return false;
                } 
                else if (!authResult.Succeeded) 
                {
                    exception = authResult.Failure ?? new InvalidOperationException("Ошибка состояния, перенаправление не возможно");
                    properties = authResult.Properties;
                }

                tiket = authResult.Tiket;
                properties = tiket.Properties;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                ///TODO: обработка в случае ошибок
            }

            await context.CommunityAuthorization(tiket);

            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                tiket.Properties.RedirectUri = "/";
            }

            Response.Redirect(properties.RedirectUri);

            return true;
        }

        private async Task<ResultAuthCommunity> HandleAuthenticationAsync() 
        {
            var query = Request.Query;

            var state = query["state"];

            var properties = options.StateDataFormat
                .Unprotect(state, Request.HttpContext.User.Identity.Name);

            if (properties is null || properties.IsEmpty) 
            {
                return ResultAuthCommunity.Fail("Состояние подключения для получения доступа отсутствует или недействительно", null);
            }

            var error = query["error"];
            if (!string.IsNullOrEmpty(error)) 
            {
                var errorDescription = query["error_description"];

                if (string.Equals(error, "access_denied")) 
                {

                    var result = HandleAccessDeniedError(properties);

                    if (!result.Skippeded) {
                        return result;
                    }

                    var deniedExeption = new Exception("Доступ был запрещен владельцем ресурса или удаленным сервером.");

                    deniedExeption.Data["error"] = error.ToString();
                    deniedExeption.Data["error_description"] = errorDescription.ToString();

                    return ResultAuthCommunity.Fail(deniedExeption, properties);
                }

                var failureMessage = new StringBuilder();

                failureMessage.Append(error);

                if (!string.IsNullOrEmpty(errorDescription)) 
                {
                    failureMessage.Append(";Description=")
                        .Append(errorDescription);
                }

                var exception = new Exception(failureMessage.ToString());

                exception.Data["error"] = error.ToString();
                exception.Data["error_description"] = errorDescription.ToString();

                return ResultAuthCommunity.Fail(exception, properties);
            }

            var code = query["code"];

            if (string.IsNullOrEmpty(code)) 
            {
                return ResultAuthCommunity.Fail("Код не найден", properties);
            }

            var codeExchangeContext
                = new CodeExchangeContext(properties, code, BuildRedirectUri(options.CallbackPath));

            var tokens = await ExchangeCodeForKey(codeExchangeContext);

            if (tokens.Error != null) 
            {
                return ResultAuthCommunity.Fail(tokens.Error, properties);
            }

            if (tokens.Tokens is null || !tokens.Tokens.Any())
            {
                return ResultAuthCommunity.Fail("Не удалось получить ключ доступа", properties);
            }

            return ResultAuthCommunity.Success(new TicketAuthCommunity
            {
                Properties = properties,
                StoreToken = tokens
            });
        }

        private async Task<TokenResponseAuthCommunity> ExchangeCodeForKey(CodeExchangeContext context) 
        {
            var tokenRequestParametrs = new Dictionary<string, string>() 
            {
                { "client_id", options.ClientId },
                { "client_secret", options.ClientSecret },
                { "redirect_uri", context.RedirectUri },
                { "code", context.Code }
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParametrs);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Version = options.Backchannel.DefaultRequestVersion;
            requestMessage.Content = requestContent;

            var response = await options.Backchannel.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode) 
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                return TokenResponseAuthCommunity.Success(payload);
            }
            else 
            {
                string errorMessage = "Сбой получения ключей доступа на конечной точке: " + await Display(response);
                return TokenResponseAuthCommunity.Failed(new Exception(errorMessage));
            }
        }

        public async Task HandleChalengeAsync(PropertiesAuthCommunity properties) 
        {
            if (string.IsNullOrEmpty(properties.RedirectUri)) 
            {
                properties.RedirectUri = Request.PathBase + Request.Path + Request.QueryString;
            }

            var authCommunityEndpoint = BuildChallengeUrl(properties, BuildRedirectUri(options.CallbackPath));

            var redirectContext = new AuthCommunityEventContext(
                properties: properties,
                options: options,
                context: context,
                challengeUrl: authCommunityEndpoint);

           await Events.RedirectToAuthorizationEndpoint(redirectContext);
        }

        private ResultAuthCommunity HandleAccessDeniedError(PropertiesAuthCommunity properties)
        {
            ///TODO: запись в лог
            
            if (options.AccessDeniedPath.HasValue)
            {
                var uri = options.AccessDeniedPath.Value;

                if (!string.IsNullOrEmpty(properties.RedirectUri))
                {
                    uri = QueryHelpers.AddQueryString(uri, "RedirectUri", properties.RedirectUri);
                }

                Response.Redirect(BuildRedirectUri(uri));
                
                return ResultAuthCommunity.Handled();
            } 

            if (!string.IsNullOrEmpty(properties.RedirectUri))
            {
                Response.Redirect(BuildRedirectUri(properties.RedirectUri));
                return ResultAuthCommunity.Handled();
            }

            return ResultAuthCommunity.Skipped();
        }

        private string BuildChallengeUrl(PropertiesAuthCommunity properties, string redirectUri) 
        {
            var scope = string.Join(",", options.Scope);
            var state = options.StateDataFormat.Protect(properties);

            var parameters = new Dictionary<string, string> 
            {
                { "client_id", options.ClientId },
                { "redirect_uri", redirectUri },
                { "group_ids", properties.CommunityId },
                { "display", "page" },
                { "scope", scope },
                { "response_type", "code" },
                { "v", options.ApiVersion },
                { "state", state }
            };

            return QueryHelpers.AddQueryString(options.AuthorizationEndpoint, parameters);
        }

        private string BuildRedirectUri(string targetPatch) 
        {
            return Request.Scheme + Uri.SchemeDelimiter + Request.Host + targetPatch;
        }

        private static async Task<string> Display(HttpResponseMessage response) 
        {
            var output = new StringBuilder();

            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers.ToString() + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");

            return output.ToString();
        }
    }
}
