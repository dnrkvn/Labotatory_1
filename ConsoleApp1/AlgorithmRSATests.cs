using System.Linq;
using NUnit.Framework;

namespace ConsoleApp1
{
    public class AlgorithmRSATests
    {
        [Test]
        [TestCase("hello", "17", "23")]
        [TestCase("hello", "101", "103")]
        [TestCase("hello", "3571", "3331")]
        [TestCase("hello", "5037569", "5810011")]
        public void CorrectDecode(string value, string number1, string number2)
        {
            var p = new MyBigInteger(number1);
            var q = new MyBigInteger(number2);
            var module = p * q;
            var phi = (p - MyBigInteger.One) * (q - MyBigInteger.One);
            var exponent = AlgorithmRSA.CalculatePublicExponent(phi);
            var secretExponent = AlgorithmRSA.CalculateSecretExponent(exponent, phi);
            var chars = value.ToCharArray().Select(x => (int)(byte)x).ToArray();
            var encoded = AlgorithmRSA.Encode(chars,
                                     exponent, module);
            var result = AlgorithmRSA.Decode(encoded, secretExponent, module);
            CollectionAssert.AreEqual(result, chars);
        }
    }
}
