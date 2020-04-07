using Castle.Core.Internal;
using DiffMatchPatch;
using Microsoft.EntityFrameworkCore;
using PlagiarismCheckingSystem.Data;
using PlagiarismCheckingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlagiarismCheckingSystem.Services
{
    public class UDiffPlagiarismDetector : IPlagiarismDetector
    {
        private Context _context;

        public UDiffPlagiarismDetector(Context context)
        {
            _context = context;
        }
        public Dictionary<LaboratoryWork, List<Similarity>> FindAndSaveSimilarities(LaboratoryWork laboratoryWork)
        {
            Dictionary<LaboratoryWork, List<Similarity>> laboratorySimilarities = new Dictionary<LaboratoryWork, List<Similarity>>();
            foreach (File sourceFile in laboratoryWork.Files)
            {
                var dmp = new diff_match_patch();
                foreach (LaboratoryWork anotherLaboratory in _context.LaboratoryWork.ToList())
                {
                    List<Similarity> similarities = new List<Similarity>();
                    foreach (File file in anotherLaboratory.Files)
                    {
                        var diffs = dmp.diff_main(file.Content, sourceFile.Content);
                        var countEquals = CountEquals(diffs);
                        decimal similarity = (countEquals / (decimal)Math.Max(sourceFile.Content.Length, file.Content.Length));
                        if (similarity >= (decimal)0.6)
                        {
                            Similarity s = new Similarity { SimilarityToId = file.Id, Value = similarity, File = sourceFile };
                            sourceFile.Similarities.Add(s);
                            similarities.Add(s);
                        }
                    }
                    if(!laboratorySimilarities.TryAdd(anotherLaboratory, similarities))
                    {
                        similarities.AddRange(laboratorySimilarities[anotherLaboratory]);
                        laboratorySimilarities[anotherLaboratory] = similarities;
                    }
                    
                }
            }

            return laboratorySimilarities;
        }

        public decimal CalculatePlagiarism(Dictionary<LaboratoryWork, List<Similarity>> laboratorySimilarities)
        {
            List<decimal> values = new List<decimal>();
            foreach(KeyValuePair<LaboratoryWork, List<Similarity>> laboratorySimilarity in laboratorySimilarities)
            {
                if (laboratorySimilarity.Value.IsNullOrEmpty()) continue;

                decimal average = laboratorySimilarity.Value.Average(keyValuePair => keyValuePair.Value);
                values.Add(average);

            }

            return values.IsNullOrEmpty() ? 0 : values.Max();
        }

        public string GeneratePrettyHtmlDiff(string source, string target)
        {
            var dmp = new diff_match_patch();
            var diffs = dmp.diff_main(source, target);

            return dmp.diff_prettyHtml(diffs);
        }

        private int CountEquals(List<Diff> diffs)
        {
            var equals = diffs.Where((Diff elem) => (elem.operation == Operation.EQUAL && !string.IsNullOrWhiteSpace(elem.text)));
            var equalsLength = equals.Aggregate(0, (acc, x) => acc + x.text.Length);

            return equalsLength;
        }
    }
}
