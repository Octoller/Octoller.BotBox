using System.Collections.Generic;
using System.Linq;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// Словарь для хранения значений состояния аутентификации сообщества.
    /// </summary>
    public class PropertiesAuthCommunity 
    {
        public const string redirectKey = ".redirectUri";
        public const string communityKey = ".community";

        public IDictionary<string, string> Items { get; }

        /// <summary>
        /// Создает новый объект <see cref="PropertiesAuthCommunity">.
        /// </summary>
        public PropertiesAuthCommunity() 
        {
            Items = new Dictionary<string, string>();
        }

        /// <summary>
        /// Создает новый объект <see cref="PropertiesAuthCommunity">.
        /// </summary>
        /// <param name="items">Словарь значений состояния.</param>
        public PropertiesAuthCommunity(IDictionary<string, string> items) 
        {
            Items = items ?? new Dictionary<string, string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Указывает, задан ли словарь значений.
        /// </summary>
        public bool IsEmpty => Items is null || !Items.Any(); 

        /// <summary>
        /// Возвращает или задает URI, который будет использоваться в качестве значения ответа перенаправления.
        /// </summary>
        public string RedirectUri 
        {
            get => GetString(redirectKey);
            set => SetString(redirectKey, value);
        }

        /// <summary>
        /// Возвращает или задаёт значение идентификатора сообщества, которое необходимо аутентифицировать.
        /// </summary>
        public string CommunityId 
        {
            get => GetString(communityKey);
            set => SetString(communityKey, value);
        }

        /// <summary>
        /// Возвращает строкове значение из коллеции <see cref="Items"/>.
        /// </summary>
        /// <param name="key">Ключ свойства.</param>
        /// <returns>Возвращает значение или <c>null</c> если значение не установлено.</returns>
        public string GetString(string key) 
            => Items.TryGetValue(key, out var value) ? value : null;

        /// <summary>
        /// Устанавливает или удаляет значение из коллекции <see cref="Items"/>.
        /// </summary>
        /// <param name="key">Ключ свойства.</param>
        /// <param name="value">Значение для установки. Если <see langword="null"/> удаляет значение по ключу.</param>
        public void SetString(string key, string value) 
        {
            if (value != null) 
            {
                Items[key] = value;
            } 
            else 
            {
                Items.Remove(key);
            }
        }
    }
}
