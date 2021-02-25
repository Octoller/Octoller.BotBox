using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// Контракт на защиту данных
    /// Написан основываясь на https://vk.cc/bYEBRC
    /// </summary>
    public class DataProtectorAuthComunnity 
    {
        private readonly IDataProtector protector;
        private readonly DataSerializerAuthCommunity serializer;

        public DataProtectorAuthComunnity(
            DataSerializerAuthCommunity serializer,
            IDataProtector protector) 
        {
            this.protector = protector;
            this.serializer = serializer;
        }

        /// <summary>
        /// Защищает переданные данные
        /// </summary>
        /// <param name="data">Данные для защиты</param>
        /// <returns>Строка защищенных данных</returns>
        public string Protect(PropertiesAuthCommunity data)
            => Protect(data, typeof(OptionsAuthCommunity).FullName);

        /// <summary>
        /// Защищает переданные данные
        /// </summary>
        /// <param name="data">Данные для защиты</param>
        /// <param name="purpose">Ключ</param>
        /// <returns>Строка защищенных данных</returns>
        public string Protect(PropertiesAuthCommunity data, string purpose) 
        {
            if (string.IsNullOrEmpty(purpose)) 
            {
                throw new ArgumentNullException(nameof(purpose));
            }

            byte[] propertyData = serializer.Serialize(data);
            byte[] protectedData = protector.Protect(propertyData);

            return Base64UrlTextEncoder.Encode(protectedData);
        }

        /// <summary>
        /// Расшифровывает ранее защищенные данные
        /// </summary>
        /// <param name="protectedText">Строка защищенных данных</param>
        /// <returns>Расшифрованный объект данных</returns>
        public PropertiesAuthCommunity Unprotect(string protectedText) 
            => Unprotect(protectedText, typeof(OptionsAuthCommunity).FullName);
        

        /// <summary>
        /// Расшифровывает ранее защищенные данные
        /// </summary>
        /// <param name="protectedText">Строка защищенных данных</param>
        /// <param name="purpose">Цель</param>
        /// <returns>Расшифрованный объект данных</returns>
        public PropertiesAuthCommunity Unprotect(string protectedText, string purpose) 
        {
            try 
            {
                if (string.IsNullOrEmpty(protectedText)) 
                {
                    return default;
                }

                var protectedData = Base64UrlTextEncoder.Decode(protectedText);

                if (protectedData is null) 
                {
                    return default;
                }

                if (string.IsNullOrEmpty(purpose)) 
                {
                    throw new ArgumentNullException(nameof(purpose));
                }

                byte[] userData = protector.Unprotect(protectedData);

                if (userData is null) 
                {
                    return default;
                }

                return serializer.Deserialize(userData);
            } 
            catch 
            {
                return default;
            }
        }
    }
}
