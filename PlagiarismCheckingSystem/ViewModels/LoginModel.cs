using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не вказано Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не вказано пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
