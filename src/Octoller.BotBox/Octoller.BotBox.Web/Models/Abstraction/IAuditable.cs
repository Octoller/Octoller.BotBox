using System;

namespace Octoller.BotBox.Web.Models.Abstraction 
{
    public interface IAuditable 
    {
        /// <summary>
        /// Указывает, когда создан объект
        /// </summary>
        public DateTime CreatedAt {  get; set; }

        /// <summary>
        /// Указывает, кем создан объект
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Указывает, когда последний раз редактировался объект
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Указывает, кем последний раз редактировался объект
        /// </summary>
        public string UpdatedBy { get; set; }
    }
}
