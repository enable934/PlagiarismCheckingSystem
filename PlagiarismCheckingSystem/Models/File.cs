using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public virtual LaboratoryWork LaboratoryWork { get; set; }
        public virtual ICollection<Similarity> Similarities { get; set; } = new List<Similarity>();
    }
}
