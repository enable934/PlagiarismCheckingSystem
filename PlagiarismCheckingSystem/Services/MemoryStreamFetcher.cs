using Microsoft.AspNetCore.Http;
using System.IO;

namespace PlagiarismCheckingSystem.Services
{
    public class MemoryStreamFetcher : IStreamFetcher
    {
        public byte[] GetArray(IFormFile formFile)
        {
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                return stream.ToArray();
            }
        }
    }
}
