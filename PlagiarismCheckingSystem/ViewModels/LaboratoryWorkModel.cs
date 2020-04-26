using PlagiarismCheckingSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlagiarismCheckingSystem.ViewModels
{
    public class LaboratoryWorkModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Не вказано Назву")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Не вибрано жодного файлу")]
        [DataType(DataType.Upload)]
        public virtual ICollection<File> Files { get; set; } = new List<File>();
        public decimal Similarity { get; set; }
    }
}
