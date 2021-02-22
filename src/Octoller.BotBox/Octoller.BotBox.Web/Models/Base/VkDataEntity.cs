namespace Octoller.BotBox.Web.Models.Base 
{
    public class VkDataEntity : UserRelation 
    {
        /// <summary>
        /// Id группы на стороне сервиса Vk
        /// </summary>
        public string VkId { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url заглавной фотографии
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// Ключ доступа
        /// </summary>
        public string AccessToken { get; set; }
    }
}
