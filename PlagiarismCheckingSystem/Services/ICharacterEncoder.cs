using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Services
{
    public interface ICharacterEncoder
    {
        public string Encode(byte[] stream);
    }
}
