using Microsoft.AspNetCore.Http;
using System.IO;

namespace PlagiarismCheckingSystem.Services
{
    public interface IStreamFetcher
    {
        public byte[] GetArray(IFormFile formFile);
    }
}
