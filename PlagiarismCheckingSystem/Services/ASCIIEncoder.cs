using System.Text;

namespace PlagiarismCheckingSystem.Services
{
    public class ASCIIEncoder : ICharacterEncoder
    {
        public string Encode(byte[] stream)
        {
            return Encoding.ASCII.GetString(stream);
        }
    }
}
