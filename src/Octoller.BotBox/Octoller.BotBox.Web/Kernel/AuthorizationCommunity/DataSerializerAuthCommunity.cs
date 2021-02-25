using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.IO;
using System;

namespace Octoller.BotBox.Web.Kernel.AuthorizationCommunity
{
    /// <summary>
    /// Сереализатор свойств подключения джля получения доступа к управлению сообществами
    /// Реализован на основе https://vk.cc/bYEyYL
    /// </summary>
    public class DataSerializerAuthCommunity : IDataSerializer<PropertiesAuthCommunity> 
    {
        private const int FormatVersion = 1;

        public static DataSerializerAuthCommunity Default { get; } = new DataSerializerAuthCommunity();

        /// <summary>
        /// Сереализует объект свойств подключения в байтовый массив
        /// </summary>
        /// <param name="item">Объектс свойст для серриализации</param>
        /// <returns></returns>
        public byte[] Serialize(PropertiesAuthCommunity item) 
        {
            using (var memory = new MemoryStream()) 
            {
                using (var writer = new BinaryWriter(memory)) 
                {
                    Write(writer, item);
                    writer.Flush();
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// Возвращает десереализованный объект свойств подключения
        /// </summary>
        /// <param name="data">Байтовый массив данных</param>
        /// <returns></returns>
        public PropertiesAuthCommunity Deserialize(byte[] data) 
        {
            using (var memory = new MemoryStream(data)) 
            {
                using (var reader = new BinaryReader(memory)) 
                {
                    return Read(reader);
                }
            }
        }

        private void Write(BinaryWriter writer, PropertiesAuthCommunity properties) 
        {
            if (writer == null) 
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (properties == null) 
            {
                throw new ArgumentNullException(nameof(properties));
            }

            //записывает версию форматирования
            writer.Write(FormatVersion);
            //записывает колличесвто сериализуемых строк
            writer.Write(properties.Items.Count);

            //построчно сереализую данные из объекта свойств
            foreach (var item in properties.Items) 
            {
                writer.Write(item.Key ?? string.Empty);
                writer.Write(item.Value ?? string.Empty);
            }
        }

        private PropertiesAuthCommunity Read(BinaryReader reader) 
        {
            if (reader is null) 
            {
                throw new ArgumentNullException(nameof(reader));
            }

            //проверяем совпадение версий
            if (reader.ReadInt32() != FormatVersion) 
            {
                return null;
            }

            //считываем колличество записаных строк
            var count = reader.ReadInt32();

            var extra = new Dictionary<string, string>(count);

            for (int i = 0; i != count; ++i) 
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                extra.Add(key, value);
            }

            return new PropertiesAuthCommunity(extra);
        }
    }
}
