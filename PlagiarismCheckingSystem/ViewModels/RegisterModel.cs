using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не вказано Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не вказано пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введено неправильно")]
        public string ConfirmPassword { get; set; }
    }
}
