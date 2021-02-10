﻿using System.ComponentModel.DataAnnotations;

namespace Octoller.BotBox.Web.ViewModels {

    public class RegisterModel : ExternalProviderModel {

        public string ReturnUrl {
            get; set;
        }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Не указан Email")]
        public string Email {
            get; set;
        }

        [Display(Name = "Passsword")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Не указан пароль")]
        public string Password {
            get; set;
        }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Required]
        public string PasswordConfirm {
            get; set;
        }

    }
}
