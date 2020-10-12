using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;

namespace kino.Crypto
{
    public class CaesarEncoder : IEncoder
    {
        private readonly int shift;

        public CaesarEncoder(int shift)
        {
            this.shift = shift;
        }


        public string Encode(string data)
        {
            return ShiftWord(data, shift);
        }

        public string Decode(string data)
        {
            return ShiftWord(data, -shift);
        }

        private string ShiftWord(string word, int shift)
        {
            return new string(word.Select(c => ShiftChar(c, shift)).ToArray());
        }

        private char ShiftChar(char c, int shift)
        {
            if (!char.IsLetter(c))
            {
                return c;
            }
            char d = char.IsUpper(c) ? 'A' : 'a';
            int a = c + shift - d;
            if (a >= 26) a -= 26;
            if (a < 0) a += 26;
            return (char)(a + d);
        }
    }
}
