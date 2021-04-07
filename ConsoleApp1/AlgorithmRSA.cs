using System.Collections.Generic;

namespace ConsoleApp1
{
    public class AlgorithmRSA
    {
        public static List<MyBigInteger> Encode(IEnumerable<int> bytes, MyBigInteger e, MyBigInteger module)
        {
            var result = new List<MyBigInteger>();
            foreach (var b in bytes)
            {
                var integer = MyBigInteger.ModPow(new MyBigInteger(b.ToString()), e, module);
                result.Add(integer);
            }
            return result;
        }

        public static int[] Decode(List<MyBigInteger> integers, MyBigInteger d, MyBigInteger module)
        {
            var result = new List<int>();
            foreach (var integer in integers)
            {
                var code = MyBigInteger.ModPow(integer, d, module);
                result.Add(int.Parse(code.ToString()));
            }
            return result.ToArray();
        }

        public static MyBigInteger CalculatePublicExponent(MyBigInteger module)
        {
            var exponent = new MyBigInteger(3);
            var one = MyBigInteger.One;

            for (var i = new MyBigInteger(0); i < module; i++)
            {
                if (MyBigInteger.GreatestCommonDivisor(exponent, module, out var _, out var _) == one)
                    return exponent;
                exponent += one;
            }
            return exponent;
        }

        public static MyBigInteger CalculateSecretExponent(MyBigInteger exponent, MyBigInteger phi)
        {
            return MyBigInteger.GetModInverse(exponent, phi);
        }
    }
}
