using System;
using System.Collections.Generic;

namespace PlagiarismCheckingSystem.Models
{
    public class LaboratoryWork
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<File> Files { get; set; } = new List<File>();
        public decimal Similarity { get; set; }
    }
}
