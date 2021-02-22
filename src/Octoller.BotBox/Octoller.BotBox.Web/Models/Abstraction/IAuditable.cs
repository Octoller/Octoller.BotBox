namespace Octoller.BotBox.Web.Models.Abstraction 
{
    public interface IAuditable 
    {
        /// <summary>
        /// Указывает, когда создан объект
        /// </summary>
        public System.DateTime CreatedAt {  get; set; }

        /// <summary>
        /// Указывает, кем создан объект
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// Указывает, когда последний раз редактировался объект
        /// </summary>
        public System.DateTime? UpdateAt { get; set; }

        /// <summary>
        /// Указывает, кем последний раз редактировался объект
        /// </summary>
        public string UpdateBy { get; set; }
    }
}
