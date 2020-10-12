using Xunit;
using kino.Crypto;

namespace kino.test
{
    public class CaesarEncoderTests
    {
        [Theory]
        [InlineData("a", "b")]
        [InlineData("abz", "bca")]
        [InlineData("A", "B")]
        public void Encodes(string input, string expected)
        {
            var encoder = new CaesarEncoder(1);

            var encoded = encoder.Encode(input);

            Assert.Equal(expected, encoded);
        }

        [Theory]
        [InlineData("a", "y")]
        [InlineData("pwa", "nuy")]
        [InlineData("A", "Y")]
        public void Decodes(string input, string expected)
        {
            var encoder = new CaesarEncoder(-2);

            var encoded = encoder.Encode(input);

            Assert.Equal(expected, encoded);
        }

        [Theory]
        [InlineData("a", 100)]
        [InlineData("Tets", 1)]
        [InlineData("Zaq12wsx!", 3)]
        public void EncodesAndDecodesToSameString(string input, int shift)
        {
            var encoder = new CaesarEncoder(shift);

            var encoded = encoder.Encode(input);
            var decoded = encoder.Decode(encoded);

            Assert.Equal(input, decoded);
        }
    }
}
