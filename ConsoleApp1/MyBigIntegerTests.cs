using NUnit.Framework;

namespace ConsoleApp1
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("255")]
        [TestCase("0")]
        [TestCase("12")]
        [TestCase("123242354656456")]
        [TestCase("-1234")]
        [TestCase("1234")]
        public void IntegerToStringCorrectly(string value)
        {
            var integer = new MyBigInteger(value);
            if (value.StartsWith('+'))
                value = value.Substring(1);
            Assert.AreEqual(value, integer.ToString());
        }

        [Test]
        [TestCase(0, TestName = "OnZero")]
        [TestCase(byte.MaxValue, TestName = "OnPositiveByte")]
        [TestCase(short.MaxValue, TestName = "OnPositiveShort")]
        [TestCase(ushort.MaxValue, TestName = "OnPositiveUShort")]
        [TestCase(int.MaxValue, TestName = "OnPositiveInt")]
        [TestCase(long.MaxValue, TestName = "OnPositiveLong")]
        [TestCase(byte.MinValue, TestName = "OnNegativeByte")]
        [TestCase(short.MinValue, TestName = "OnNegativeShort")]
        [TestCase(ushort.MinValue, TestName = "OnNegativeUShort")]
        [TestCase(int.MinValue, TestName = "OnNegativeInt")]
        [TestCase(long.MinValue, TestName = "OnNegativeLong")]
        public void IntegerShouldGetRightToString(long value)
        {
            var integer = new MyBigInteger(value.ToString());
            Assert.AreEqual(value.ToString(), integer.ToString());
        }

        [Test]
        [TestCase(byte.MaxValue, 2, byte.MaxValue + 2, TestName = "OverPositiveByte")]
        [TestCase(byte.MinValue, -2, byte.MinValue - 2, TestName = "OverNegativeByte")]
        [TestCase(short.MaxValue, 2, short.MaxValue + 2, TestName = "OverPositiveShort")]
        [TestCase(short.MinValue, -2, short.MinValue - 2, TestName = "OverNegativeShort")]
        public void IntegerShouldSumCorrectly(long a, long b, long expected)
        {
            var actual = new MyBigInteger(a.ToString()) + new MyBigInteger(b.ToString());
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [Test]
        [TestCase(int.MaxValue, 2, "2147483649", TestName = "OverPositiveInt")]
        [TestCase(int.MinValue, -2, "-2147483650", TestName = "OverNegativeInt")]
        [TestCase(long.MaxValue, 2, "9223372036854775809")]
        [TestCase(long.MinValue, -2, "-9223372036854775810")]
        public void IntegerOverLongShouldSumCorrectly(long a, long b, string expected)
        {
            var actual = new MyBigInteger(a.ToString()) + new MyBigInteger(b.ToString());
            Assert.AreEqual(expected, actual.ToString());
        }

        [Test]
        [TestCase(byte.MaxValue, byte.MinValue)]
        [TestCase(int.MaxValue, int.MinValue)]
        [TestCase(short.MaxValue, short.MinValue)]
        [TestCase(long.MaxValue, long.MinValue)]
        [TestCase(short.MaxValue, short.MaxValue)]
        [TestCase(byte.MaxValue, byte.MaxValue)]
        [TestCase(int.MaxValue, int.MaxValue)]
        [TestCase(1111, 1112)]
        [TestCase(-1111, 1112)]
        public void IntegerShouldCorrectlyCompare(long first, long second)
        {
            Assert.AreEqual(first < second,
                            new MyBigInteger(first.ToString()) < new MyBigInteger(second.ToString()));
        }

        [Test]
        [TestCase(byte.MaxValue)]
        [TestCase(byte.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        [TestCase(short.MinValue)]
        [TestCase(short.MinValue)]
        public void IntegerShouldEqualCorrectly(long value)
        {
            var first = new MyBigInteger(value.ToString());
            var second = new MyBigInteger(value.ToString());
            Assert.That(first == second, Is.True);
        }

        [Test]
        [TestCase("12", "-12", "0")]
        [TestCase("12", "-2", "10")]
        [TestCase("1222222222222", "-2", "1222222222220")]
        [TestCase("1223324345364564563524354234536", "-2", "1223324345364564563524354234534")]
        [TestCase("1000", "-1", "999")]
        [TestCase("1000", "-999", "1")]
        [TestCase("-1000", "999", "-1")]
        public void IntegerShouldSubtractBySumCorrectly(string first, string second,
                                                        string expected)
        {
            var sum = new MyBigInteger(first) + new MyBigInteger(second);
            Assert.AreEqual(expected, sum.ToString());
        }

        [Test]
        [TestCase("10", "2", "8")]
        [TestCase("-10", "-2", "-8")]
        [TestCase("10", "-2", "12")]
        [TestCase("-10", "2", "-12")]
        [TestCase("1234254525677895465425345", "31324462654746584234", "1234223201215240718841111")]
        [TestCase("1000", "1", "999")]
        [TestCase("101", "2", "99")]
        [TestCase("99", "-2", "101")]
        public void IntegerShouldSubtractCorrectly(string first, string second, string expected)
        {
            var sub = new MyBigInteger(first) - new MyBigInteger(second);
            Assert.AreEqual(sub.ToString(), expected);
        }

        [Test]
        [TestCase("2", "2", "4")]
        [TestCase("-2", "2", "-4")]
        [TestCase("2", "-2", "-4")]
        [TestCase("-2", "-2", "4")]
        [TestCase("-2", "0", "0")]
        [TestCase("0", "-2", "0")]
        [TestCase("123234253577675484345657", "0", "0")]
        [TestCase("123234253577675484345657", "1", "123234253577675484345657")]
        [TestCase("-123234253577675484345657", "-1", "123234253577675484345657")]
        [TestCase("-123234253577675484345657", "3152435", "-388487974177139415493201224795")]
        public void IntegerShouldMultiplyCorrectly(string first, string second, string expected)
        {
            Assert.AreEqual(expected, (new MyBigInteger(first) * new MyBigInteger(second)).ToString());
        }

        [Test]
        [TestCase("4", "2", "2")]
        [TestCase("1", "2", "0")]
        [TestCase("10000", "10", "1000")]
        [TestCase("180", "60", "3")]
        [TestCase("18", "6", "3")]
        [TestCase("10000000", "10", "1000000")]
        [TestCase("1232347", "315", "3912")]
        [TestCase("123234253577675484345657", "3152435", "39091766706585697")]
        public void IntegerShouldDivCorrectly(string first, string second, string expected)
        {
            var div = new MyBigInteger(first) / new MyBigInteger(second);
            Assert.AreEqual(expected, div.ToString());
        }

        [Test]
        [TestCase("10", "1", "0")]
        [TestCase("10000", "10", "0")]
        [TestCase("12", "6", "0")]
        [TestCase("1232347", "315", "67")]
        [TestCase("123234365457376547", "34675869678915", "31000488191552")]
        public void IntegerShouldModCorrectly(string first, string second, string expected)
        {
            Assert.AreEqual(expected, (new MyBigInteger(first) % new MyBigInteger(second)).ToString());
        }

        [Test]
        [TestCase("2", "2", "4")]
        [TestCase("2", "3", "8")]
        [TestCase("-2", "3", "-8")]
        [TestCase("-2", "2", "4")]
        [TestCase("1234", "4", "2318785835536")]
        [TestCase("1221323434", "42",
                     "4434952639964279782616805006137505628734689158636958537963449162698885920764334703019598003062207553806465876421936943638342717966040413589628179790809025899456885228096871460630949699667890185920933384226837514357330927243376257707168806555648152848717395982802190348143837315645586200671747503241120002484398277556330789617761417955822901031644010057562118850889160615115626643456")]
        public void IntegerShouldPowCorrectly(string value, string power, string expected)
        {
            var result = MyBigInteger.Pow(new MyBigInteger(value), new MyBigInteger(power)).ToString();
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("332453453", "26", "15")]
        [TestCase("3323", "33", "23")]
        [TestCase("332354656", "33", "19")]
        [TestCase("332354656", "1", "0")]
        [TestCase("5", "12", "5")]
        public void IntegerShouldModInverseCorrectly(string first, string second, string expected)
        {
            var result = MyBigInteger.GetModInverse(new MyBigInteger(first), new MyBigInteger(second)).ToString();
            Assert.AreEqual(expected, result);
        }
    }
}
