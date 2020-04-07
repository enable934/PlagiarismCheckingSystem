using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Models
{
    public class Similarity
    {
        public int Id { get; set; }
        public int SimilarityToId { get; set; }
        public decimal Value { get; set; }
        public virtual File File { get;set; }
    }
}
