using DiffMatchPatch;
using Microsoft.EntityFrameworkCore;
using PlagiarismCheckingSystem.Models;
using System.Collections.Generic;

namespace PlagiarismCheckingSystem.Services
{
    public interface IPlagiarismDetector
    {
        public Dictionary<LaboratoryWork, List<Similarity>> FindAndSaveSimilarities(LaboratoryWork laboratoryWork);
        public decimal CalculatePlagiarism(Dictionary<LaboratoryWork, List<Similarity>> laboratorySimilarities);

        public string GeneratePrettyHtmlDiff(string source, string target);
    }
}
