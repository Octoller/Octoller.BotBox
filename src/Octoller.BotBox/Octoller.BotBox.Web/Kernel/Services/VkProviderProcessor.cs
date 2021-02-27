using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Data.Models;
using Octoller.BotBox.Web.Data.Stores;
using Octoller.BotBox.Web.Kernel.AuthorizationCommunity;
using Octoller.BotBox.Web.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkNet.Model;
using System.Linq;
using System;
using VkNet;

namespace Octoller.BotBox.Web.Kernel.Services
{
    public class VkProviderProcessor 
    {
        private readonly IVkApi vkApi;
        private readonly ILogger<VkProviderProcessor> logger;
        private readonly AccountStore accountStore;
        private readonly CommunityStore communityStore;
        private readonly UserManager<Data.Models.User> userManager;

        public VkProviderProcessor(
            ILogger<VkProviderProcessor> logger,
            CommunityStore communityStore,
            AccountStore accountStore,
            UserManager<Data.Models.User> userManager) 
        {
            this.logger = logger;
            this.accountStore = accountStore;
            this.communityStore = communityStore;
            this.userManager = userManager;
            this.vkApi = new VkApi();
        }

        /// <summary>
        /// Возвращает преобразованные данные о аккаунте пользователя на стороне вк
        /// </summary>
        /// <typeparam name="TVkData">Тип в который будет преобразован набор данных</typeparam>
        /// <param name="userId">Id пользователя</param>
        /// <param name="cast">Метод преобразования</param>
        /// <returns>Набор данных пользователя</returns>
        public async Task<Account> FindAccounByUserIdAsync(string userId) 
        {
            var vkAccount = await accountStore.GetByUserIdAsync(userId);

            if (vkAccount is null)
            {
                return default(Account);
            }
                
            return vkAccount;
        }

        /// <summary>
        /// Создает запись с данными пользователя, полученными со стороны вк
        /// </summary>
        /// <param name="loginInfo">Данные аутентификации, полученные со стороны сервиса Vk</param>
        /// <returns>Результат выполнения операции</returns>
        public async Task<IdentityResult> CreateVkAccountAsync(string userId, string email, ExternalLoginInfo loginInfo) 
        {
            if ((await FindAccounByUserIdAsync(userId))?.Id != null) 
            {
                return IdentityResult.Success;
            }

            var token = loginInfo.AuthenticationTokens.
                    Where(at => at.Name.Equals("access_token"))
                    ?.FirstOrDefault();

            var vkId = long.Parse(loginInfo.ProviderKey);

            try 
            {
                await vkApi?.AuthorizeAsync(new ApiAuthParams 
                {
                    AccessToken = token.Value,
                    UserId = vkId
                });

                LogWriter.Conected(logger, "Vk", $"client id {vkId}");
            } 
            catch (Exception ex) 
            {
                LogWriter.Conected(logger, "Vk", $"client id {vkId}", ex);
                return FailedResult(ex.Message);
            }

            var vkUser = (await vkApi.Users.GetAsync(
                    userIds: new[] { vkId },
                    fields: ProfileFields.Photo100))
                .FirstOrDefault();

            var account = new Account 
            {
                VkId = vkId.ToString(),
                Name = vkUser.FirstName + " " + vkUser.LastName ?? " ",
                AccessToken = token.Value,
                Photo = await GetFileByteArray(vkUser.Photo100.AbsoluteUri),
                UserId = userId
            };

            var initiatorName = GetUserName(userId);

            var resultCreateAccount = await accountStore.CreateAsync(account, initiatorName);

            if (resultCreateAccount) 
            {
                var communities = await vkApi.Groups.GetAsync(new GroupsGetParams 
                {
                    UserId = vkId,
                    Filter = GroupsFilters.Administrator,
                    Fields = GroupsFields.MembersCount,
                    Extended = true
                });

                if (communities.Any()) 
                {
                    foreach (var g in communities)
                        await communityStore.CreateAsync(new Community 
                        {
                            VkId = g.Id.ToString(),
                            Name = g.Name,
                            Photo = await GetFileByteArray(g.Photo100.AbsoluteUri),
                            UserId = userId
                        }, initiatorName);
                }

                return IdentityResult.Success;
            }

            return FailedResult("Ошибка записи");
        }

        /// <summary>
        /// Возвращает список сообществ пользователя на стороне vk
        /// </summary>
        /// <typeparam name="TElement">Тип в который будет преобразован набор данных</typeparam>
        /// <param name="userId">Id пользователя</param>
        /// <param name="cast">Метод преобразования</param>
        /// <returns>Коллекция сообществ</returns>
        public IEnumerable<Community> GetCommunity(string userId) 
        {
            var groups = communityStore.GetByUserId(userId);
            var listResult = new List<Community>();

            if (groups.Any()) 
            {
                foreach (var g in groups) 
                {
                    listResult.Add(g);
                }
            }

            return listResult;
        }

        /// <summary>
        /// Конфигурирует свойства для вызова аутентификации сообщества на стороне сервиса вк
        /// </summary>
        /// <param name="id">ID сообщества</param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<PropertiesAuthCommunity> GetRequestUrlForAuthorizeCommunityAsync(string id, string redirectUri) 
        {
            var community = await communityStore.GetByIdAsync(id);

            return new PropertiesAuthCommunity 
            {
                RedirectUri = redirectUri,
                CommunityId = community.VkId
            };
        }

        /// <summary>
        /// Производит аутентификацию сообщества по ключу аутентификации
        /// </summary>
        /// <param name="tiket"></param>
        /// <returns></returns>
        public async Task CommunityAuthorizationAsync(TicketAuthCommunity tiket) 
        {
            if (tiket is null)
            {
                ///TODO: Запись ошибки в лог
                return;
            }
            
            foreach (var token in tiket.StoreToken.Tokens)
            {
               var community = communityStore.Communities
                    .Where(g => g.VkId == token.Community)
                    .FirstOrDefault();

                if (community != null)
                {
                    community.AccessToken = token.Value;
                    community.Connected = true;
                    ///TODO: при авторизации нужно указывать имя или id пользователя
                    ///TODO: Запись в лог
                    await communityStore.UpdateAsync(community);
                }
            }

            await Task.CompletedTask;
        }

        public async Task CommunityUnAuthorizationAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ///TODO: лог ошибки
                return;
            }

            var community = await communityStore.GetByIdAsync(id);

            if (community is null)
            {
                ///TODO: лог ошибки
                return;
            }

            community.AccessToken = null;
            community.Connected = false;

            var result = await communityStore.UpdateAsync(community);

            if (!result)
            {
                ///TODO: лог ошибки
                return;
            }
        }

        private static IdentityResult FailedResult(string description) 
        {
            return IdentityResult.Failed(new[] 
            { 
                new IdentityError 
                {
                    Code = "",
                    Description = description
                }
            });
        }

        private static async Task<byte[]> GetFileByteArray(string url) 
        {
            using HttpContent content = (await new HttpClient().GetAsync(url)).Content;
            return await content.ReadAsByteArrayAsync();
        }

        private string GetUserName(string id) => userManager.FindByIdAsync(id).Result.UserName;
    }
}
