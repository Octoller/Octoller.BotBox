﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Data.Stores;
using Octoller.BotBox.Web.Kernel.AuthenticationCommunity;
using Octoller.BotBox.Web.Logging;
using Octoller.BotBox.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VkNet;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace Octoller.BotBox.Web.Kernel.Services 
{
    public class VkProviderProcessor 
    {
        private readonly IVkApi vkApi;
        private readonly ILogger<VkProviderProcessor> logger;
        private readonly AccountStore accountStore;
        private readonly CommunityStore communityStore;
        private readonly UserManager<Models.User> userManager;

        public VkProviderProcessor(
            ILogger<VkProviderProcessor> logger,
            CommunityStore communityStore,
            AccountStore accountStore,
            UserManager<Models.User> userManager) 
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
        public async Task<TVkData> FindAccounByUserIdAsync<TVkData>(string userId, System.Func<Account, TVkData> cast) 
        {
            Account vkAccount = await accountStore.GetByUserIdAsync(userId);

            if (vkAccount is null)
            {
                return default(TVkData);
            }
                
            return cast(vkAccount);
        }

        /// <summary>
        /// Создает запись с данными пользователя, полученными со стороны вк
        /// </summary>
        /// <param name="loginInfo">Данные аутентификации, полученные со стороны сервиса Vk</param>
        /// <returns>Результат выполнения операции</returns>
        public async Task<IdentityResult> CreateVkAccountAsync(string userId, string email, ExternalLoginInfo loginInfo) 
        {
            if (await FindAccounByUserIdAsync(userId, a => a.Id) != null) 
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

            Account account = new Account 
            {
                VkId = vkId.ToString(),
                Name = vkUser.FirstName + " " + vkUser.LastName ?? " ",
                AccessToken = token.Value,
                Photo = await GetFileByteArray(vkUser.Photo100.AbsoluteUri),
                UserId = userId
            };

            string initiatorName = GetUserName(userId);

            bool resultCreateAccount = await accountStore.CreateAsync(account, initiatorName);

            if (resultCreateAccount) 
            {
                VkCollection<Group> communities = await vkApi.Groups.GetAsync(new GroupsGetParams 
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
        public IEnumerable<TElement> GetCommunity<TElement>(string userId, System.Func<Community, TElement> cast) 
        {
            var groups = communityStore.GetByUserId(userId);
            var listResult = new List<TElement>();

            if (groups.Any()) 
            {
                foreach (var g in groups) 
                {
                    listResult.Add(cast(g));
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
            Community community = await communityStore.GetByIdAsync(id);

            return new PropertiesAuthCommunity 
            {
                RedirectUri = redirectUri,
                CommunityId = community.VkId
            };
        }

        public async Task CommunityAuthenticateAsync(TicketAuthCommunity tiket) 
        {
            if (tiket is null)
            {
                ///TODO: Запись ошибки в лог
                await Task.CompletedTask;
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