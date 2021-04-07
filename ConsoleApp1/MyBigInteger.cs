using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public enum Sign
    {
        Minus = -1,
        Plus = 1
    }

    public class MyBigInteger
    {
        private readonly List<byte> digits; // Разбиваем длинное число на отдельные цифры, которые будут хранится в списке
        public bool IsZero => digits.Count == 0 || digits.All(x => x == 0) || this == Zero;
        public static MyBigInteger Zero => new MyBigInteger(0);
        public static MyBigInteger One => new MyBigInteger(1);

        private int Size => digits.Count;

        private Sign Sign { get; set; }

        private MyBigInteger(Sign sign, List<byte> bytes)
        {
            Sign = Sign.Plus;
            Sign = sign;
            digits = bytes.ToList();
            RemoveNulls();
        }

        // Парсинг числа из строки
        public MyBigInteger(string s)
        {
            digits = new List<byte>();
            Sign = Sign.Plus;
            if (s.StartsWith("-"))
            {
                Sign = Sign.Minus;
                s = s.Substring(1);
            }
            else Sign = Sign.Plus;

            foreach (var c in s.Reverse())
            {
                digits.Add(Convert.ToByte(c.ToString()));
            }

            RemoveNulls();
        }

        public MyBigInteger(uint x)
        {
            digits = new List<byte>();
            Sign = Sign.Plus;
            Sign = Sign.Plus;
            digits.AddRange(GetBytes(x));
        }

        private MyBigInteger(int x)
        {
            digits = new List<byte>();
            Sign = Sign.Plus;
            if (x < 0)
            {
                Sign = Sign.Minus;
            }

            digits.AddRange(GetBytes((uint)Math.Abs(x)));
        }

        // Получение списка цифр из целого беззнакового числа
        private static List<byte> GetBytes(uint num)
        {
            var bytes = new List<byte>();
            do
            {
                bytes.Add((byte)(num % 10));
                num /= 10;
            } while (num > 0);

            return bytes;
        }

        //Удаление лидирующих нулей длинного числа
        private void RemoveNulls()
        {
            var index = digits.Count - 1;
            while (index >= 0 && digits[index] == 0)
            {
                digits.RemoveAt(index);
                index--;
            }
        }

        // Получение больших чисел формата valEexp (например 1E3 = 1000)
        private static MyBigInteger Exp(byte val, int exp)
        {
            var bigInt = Zero;
            bigInt.SetByte(exp, val);
            bigInt.RemoveNulls();
            return bigInt;
        }

        // Получение цифры по индексу
        private byte GetByte(int i)
        {
            return i < Size ? digits[i] : (byte)0;
        }

        // Установка цифры по индексу
        private void SetByte(int i, byte b)
        {
            while (digits.Count <= i)
            {
                digits.Add(0);
            }

            digits[i] = b;
        }

        // Преобразование длинного числа в строку
        public override string ToString()
        {
            RemoveNulls();

            if (IsZero) return "0";
            var builder = new StringBuilder(Sign == Sign.Plus ? "" : "-");

            for (var i = digits.Count - 1; i >= 0; i--)
            {
                builder.Append(Convert.ToString(digits[i]));
            }
            return builder.ToString();
        }

        #region Методы арифметических действий над большими числами

        private static MyBigInteger Add(MyBigInteger a, MyBigInteger b)
        {
            var digits = new List<byte>();
            var maxLength = Math.Max(a.Size, b.Size);
            var reduce = 0;
            for (var i = 0; i < maxLength; i++)
            {
                var sum = (byte)(a.GetByte(i) + b.GetByte(i) + reduce);
                reduce = sum / 10;
                digits.Add((byte)(sum % 10));
            }

            if (reduce > 0)
                digits.Add((byte)reduce);

            return new MyBigInteger(a.Sign, digits);
        }

        private static MyBigInteger Subtract(MyBigInteger a, MyBigInteger b)
        {
            var digits = new List<byte>();
            var max = Zero;
            var min = Zero;
            var compare = Comparison(a, b, ignoreSign: true);

            switch (compare)
            {
                case -1:
                    min = a;
                    max = b;
                    break;
                case 0:
                    return Zero;
                case 1:
                    min = b;
                    max = a;
                    break;
            }

            var maxLength = Math.Max(a.Size, b.Size);

            var reduce = 0;
            for (var i = 0; i < maxLength; i++)
            {
                var sub = max.GetByte(i) - min.GetByte(i) - reduce;
                if (sub < 0)
                {
                    sub += 10;
                    reduce = 1;
                }
                else
                {
                    reduce = 0;
                }

                digits.Add((byte)sub);
            }

            return new MyBigInteger(max.Sign, digits);
        }

        private static MyBigInteger Multiply(MyBigInteger a, MyBigInteger b)
        {
            var result = Zero;

            for (var i = 0; i < a.Size; i++)
            {
                for (int j = 0, reduce = 0; (j < b.Size) || (reduce > 0); j++)
                {
                    var sum = result.GetByte(i + j) + a.GetByte(i) * b.GetByte(j) + reduce;
                    result.SetByte(i + j, (byte)(sum % 10));
                    reduce = sum / 10;
                }
            }

            result.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            return result;
        }

        private static MyBigInteger Div(MyBigInteger a, MyBigInteger b)
        {
            var result = Zero;
            var current = Zero;

            for (var i = a.Size - 1; i >= 0; i--)
            {
                current += Exp(a.GetByte(i), i);

                var x = 0;
                var left = 0;
                var right = 10;
                while (left <= right)
                {
                    var middle = (left + right) / 2;
                    var cur = b * Exp((byte)middle, i);
                    if (cur <= current)
                    {
                        x = middle;
                        left = middle + 1;
                    }
                    else
                    {
                        right = middle - 1;
                    }
                }

                result.SetByte(i, (byte)(x % 10));
                var reduce = b * Exp((byte)x, i);
                current -= reduce;
            }

            result.RemoveNulls();
            result.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            return result;
        }

        private static MyBigInteger Mod(MyBigInteger a, MyBigInteger b)
        {
            var result = Zero;

            for (var i = a.Size - 1; i >= 0; i--)
            {
                result += Exp(a.GetByte(i), i);

                var x = 0;
                var left = 0;
                var right = 10;

                while (left <= right)
                {
                    var middle = (left + right) >> 1;
                    var current = b * Exp((byte)middle, i);
                    if (current <= result)
                    {
                        x = middle;
                        left = middle + 1;
                    }
                    else
                    {
                        right = middle - 1;
                    }
                }

                result -= b * Exp((byte)x, i);
            }

            result.RemoveNulls();

            result.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            return result;
        }

        public static MyBigInteger ModPow(MyBigInteger value, MyBigInteger power, MyBigInteger module)
        {
            var binaryValue = ConvertToBinary(power);

            var arr = new MyBigInteger[binaryValue.Count];
            arr[0] = value;
            for (var i = 1; i < binaryValue.Count; i++)
                arr[i] = arr[i - 1] * arr[i - 1] % module;

            var multiplication = One;
            var zero = Zero;
            for (var j = 0; j < binaryValue.Count; j++)
                if (binaryValue[j] > zero)
                    multiplication *= binaryValue[j] * arr[j];

            return multiplication % module;
        }

        public static MyBigInteger Pow(MyBigInteger value, MyBigInteger power)
        {
            var two = new MyBigInteger(2);
            if (power.IsZero)
                return One;
            if (power == One)
                return value;
            if (power % two == One)
                return value * Pow(value, power - One);
            var b = Pow(value, power / two);
            return b * b;
        }

        private static List<MyBigInteger> ConvertToBinary(MyBigInteger value)
        {
            var copy = new MyBigInteger(value.Sign, value.digits.ToList());
            var two = new MyBigInteger(2);
            var result = new List<MyBigInteger>();
            while (!copy.IsZero)
            {
                result.Add(copy % two);
                copy /= two;
            }

            return result;
        }

        public static MyBigInteger GetModInverse(MyBigInteger a, MyBigInteger n)
        {
            var gdc = GreatestCommonDivisor(a, n, out var x, out var _);
            if (gdc != One)
                return Zero;
            return (x % n + n) % n;
        }

        public static MyBigInteger GreatestCommonDivisor(MyBigInteger number, MyBigInteger modulo, out MyBigInteger x,
                                                    out MyBigInteger y)
        {
            if (number.IsZero)
            {
                x = Zero;
                y = One;
                return modulo;
            }

            var d = GreatestCommonDivisor(modulo % number, number, out var x1, out var y1);
            x = y1 - (modulo / number) * x1;
            y = x1;
            return d;
        }

        #endregion

        #region Методы для сравнения больших чисел

        private static int Comparison(MyBigInteger a, MyBigInteger b, bool ignoreSign = false)
        {
            return CompareSign(a, b, ignoreSign);
        }

        private static int CompareSign(MyBigInteger a, MyBigInteger b, bool ignoreSign = false)
        {
            if (!ignoreSign)
            {
                if (a.Sign < b.Sign)
                {
                    return -1;
                }

                if (a.Sign > b.Sign)
                {
                    return 1;
                }
            }

            return CompareSize(a, b);
        }

        private static int CompareSize(MyBigInteger a, MyBigInteger b)
        {
            if (a.Size < b.Size)
            {
                return -1;
            }
            else if (a.Size > b.Size)
            {
                return 1;
            }

            return CompareDigits(a, b);
        }

        private static int CompareDigits(MyBigInteger a, MyBigInteger b)
        {
            var maxLength = Math.Max(a.Size, b.Size);
            for (var i = maxLength; i >= 0; i--)
            {
                if (a.GetByte(i) < b.GetByte(i))
                {
                    return -1;
                }
                if (a.GetByte(i) > b.GetByte(i))
                {
                    return 1;
                }
            }

            return 0;
        }

        #endregion

        #region Арифметические операторы

        // Унарный минус (изменение знака числа)
        public static MyBigInteger operator -(MyBigInteger a)
        {
            a.Sign = a.Sign == Sign.Plus ? Sign.Minus : Sign.Plus;
            return a;
        }

        public static MyBigInteger operator +(MyBigInteger a, MyBigInteger b) => a.Sign == b.Sign ? Add(a, b) : Subtract(a, b);

        public static MyBigInteger operator -(MyBigInteger a, MyBigInteger b) => a + -b;

        public static MyBigInteger operator *(MyBigInteger a, MyBigInteger b) => Multiply(a, b);

        public static MyBigInteger operator /(MyBigInteger a, MyBigInteger b) => Div(a, b);

        public static MyBigInteger operator %(MyBigInteger a, MyBigInteger b) => Mod(a, b);

        public static MyBigInteger operator ++(MyBigInteger integer) => integer + One;

        public static MyBigInteger operator --(MyBigInteger integer) => integer - One;

        #endregion

        #region Операторы сравнения

        public static bool operator <(MyBigInteger a, MyBigInteger b) => Comparison(a, b) < 0;

        public static bool operator >(MyBigInteger a, MyBigInteger b) => Comparison(a, b) > 0;

        public static bool operator <=(MyBigInteger a, MyBigInteger b) => Comparison(a, b) <= 0;

        public static bool operator >=(MyBigInteger a, MyBigInteger b) => Comparison(a, b) >= 0;

        public static bool operator ==(MyBigInteger a, MyBigInteger b) => Comparison(a, b) == 0;

        public static bool operator !=(MyBigInteger a, MyBigInteger b) => Comparison(a, b) != 0;

        public override bool Equals(object obj) => obj is MyBigInteger integer && this == integer;

        #endregion
    }
}
