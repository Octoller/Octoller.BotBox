using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Octoller.BotBox.Web.Data.Stores;
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

namespace Octoller.BotBox.Web.Kernel.Processor {

    public class VkProviderProcessor {

        private readonly IVkApi vkApi;
        private readonly ILogger<VkProviderProcessor> logger;
        private readonly VkDataStore store;

        public VkProviderProcessor(
            ILogger<VkProviderProcessor> logger,
            VkDataStore vkStore) {

            this.logger = logger;
            this.store = vkStore;
            this.vkApi = new VkApi();
        }

        /// <summary>
        /// Возвращает преобразованные данные о аккаунте пользователя на стороне вк
        /// </summary>
        /// <typeparam name="TVkData">Тип в который будет преобразован набор данных</typeparam>
        /// <param name="userId">Id пользователя</param>
        /// <param name="cast">Метод преобразования</param>
        /// <returns>Набор данных пользователя</returns>
        public async Task<TVkData> FindAccounByUserIdAsync<TVkData>(string userId, System.Func<Account, TVkData> cast) {

            Account vkAccount = await this.store.GetAccountByUserIdAsync(userId);

            if (vkAccount is null)
                return default(TVkData);

            return cast(vkAccount);
        }

        /// <summary>
        /// Создает запись с данными пользователя, полученными со стороны вк
        /// </summary>
        /// <param name="loginInfo">Данные аутентификации, полученные со стороны сервиса Vk</param>
        /// <returns>Результат выполнения операции</returns>
        public async Task<IdentityResult> CreateVkAccountAsync(string userId, string email, ExternalLoginInfo loginInfo) {

            if (await this.FindAccounByUserIdAsync(userId, a => a.Id) is not null) {
                return IdentityResult.Success;
            }

            var token = loginInfo.AuthenticationTokens.
                    Where(at => at.Name.Equals("access_token"))
                    ?.FirstOrDefault();

            var vkId = long.Parse(loginInfo.ProviderKey);

            try {

                await this.vkApi?.AuthorizeAsync(new VkNet.Model.ApiAuthParams {
                    AccessToken = token.Value,
                    UserId = vkId
                });

                LogWriter.Conected(logger, "Vk", $"client id {vkId}");

            } catch (System.Exception ex) {

                LogWriter.Conected(logger, "Vk", $"client id {vkId}", ex);

                return FailedResult(ex.Message);
            }

            var vkUser = (await this.vkApi.Users.GetAsync(
                    userIds: new[] { vkId },
                    fields: ProfileFields.Photo100))
                .FirstOrDefault();

            Account account = new Account {
                VkId = vkId.ToString(),
                Name = vkUser.FirstName + " " + vkUser.LastName ?? " ",
                AccessToken = token.Value,
                Photo = await GetFileByteArray(vkUser.Photo100.AbsoluteUri),
                UpdateAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdateBy = email,
                CreateBy = email,
                UserId = userId
            };

            bool resultCreateAccount = await this.store.CreateAccountAsync(account);

            if (resultCreateAccount) {

                VkCollection<Group> communities = await this.vkApi.Groups.GetAsync(new GroupsGetParams {
                    UserId = vkId,
                    Filter = GroupsFilters.Administrator,
                    Fields = GroupsFields.MembersCount,
                    Extended = true
                });

                if (communities.Any()) {

                    foreach (var g in communities)
                        await this.store.CreateGroupAsync(new Community {
                            VkId = g.Id.ToString(),
                            Name = g.Name,
                            Photo = await GetFileByteArray(g.Photo100.AbsoluteUri),
                            UpdateAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            UpdateBy = email,
                            CreateBy = email,
                            UserId = userId
                        });
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
        public IEnumerable<TElement> GetCommunity<TElement>(string userId, System.Func<Community, TElement> cast) {

            var groups = this.store.GetGroupByUserId(userId);
            var listResult = new List<TElement>();

            if (groups.Any()) {
                foreach (var g in groups) {
                    listResult.Add(cast(g));
                }
            }

            return listResult;
        }

        private static IdentityResult FailedResult(string description) {

            return IdentityResult.Failed(new[] { new IdentityError {
                    Code = "",
                    Description = description
                }
            });
        }

        private static async Task<byte[]> GetFileByteArray(string url) {

            using HttpContent content = (await new HttpClient().GetAsync(url)).Content;

            return await content.ReadAsByteArrayAsync();
        }
    }
}
