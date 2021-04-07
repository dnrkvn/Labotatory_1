using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Enter command (available commands is decode, encode and encodeFile):");
            var line = Console.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                switch (line.Trim())
                {
                    case "encode":
                        Encode();
                        break;
                    case "decode":
                        Decode();
                        break;
                    case "encodeFile":
                        EncodeFile();
                        break;
                    default:
                        Console.WriteLine("Available commands is decode, encode and encodeFile");
                        break;
                }
                Console.WriteLine($"Enter command: ");
                line = Console.ReadLine();
            }
        }

        private static void Encode()
        {
            Console.Write("Enter first prime number: ");
            var p = new MyBigInteger(Console.ReadLine());

            Console.Write("Enter second prime number: ");
            var q = new MyBigInteger(Console.ReadLine());

            var module = p * q;
            Console.WriteLine($"Your module: {module}");

            var phi = (p - MyBigInteger.One) * (q - MyBigInteger.One);
            var publicExponent = AlgorithmRSA.CalculatePublicExponent(phi);
            var secretExponent = AlgorithmRSA.CalculateSecretExponent(publicExponent, phi);
            Console.WriteLine($"Your secret exponent: {secretExponent}");

            Console.WriteLine("Enter your message:");
            var message = Encoding.ASCII
                                  .GetBytes(Console.ReadLine() ?? string.Empty)
                                  .Select(x => (int)x)
                                  .ToArray();

            var encode = AlgorithmRSA.Encode(message, publicExponent, module);

            Console.WriteLine("Your crypted data:");
            foreach (var integer in encode)
            {
                Console.WriteLine(integer);
            }
        }

        private static void Decode()
        {
            Console.WriteLine("Enter your module: ");
            var module = new MyBigInteger(Console.ReadLine());

            Console.WriteLine("Enter your secret exponent: ");
            var secretExponent = new MyBigInteger(Console.ReadLine());

            Console.WriteLine("Enter your crypted data per line: ");
            var crypted = new List<MyBigInteger>();
            var line = Console.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                crypted.Add(new MyBigInteger(line));
                line = Console.ReadLine();
            }

            var chars = AlgorithmRSA.Decode(crypted, secretExponent, module);
            var result = Encoding.ASCII.GetString(chars.Select(x => (byte)x).ToArray());
            Console.WriteLine($"Your message was {result}");
        }

        private static void EncodeFile()
        {
            Console.Write("Enter first prime number: ");
            var p = new MyBigInteger(Console.ReadLine());

            Console.Write("Enter second prime number: ");
            var q = new MyBigInteger(Console.ReadLine());

            var module = p * q;
            Console.WriteLine($"Your module: {module}");

            var phi = (p - MyBigInteger.One) * (q - MyBigInteger.One);
            var publicExponent = AlgorithmRSA.CalculatePublicExponent(phi);
            var secretExponent = AlgorithmRSA.CalculateSecretExponent(publicExponent, phi);
            Console.WriteLine($"Your secret exponent: {secretExponent}");

            var message =
                File.ReadAllBytes(@"C:\Users\diyll\Laboratory\ConsoleApp1\ConsoleApp1\FileToEncode.txt")
                    .Select(x => (int)x);
            var encoded = AlgorithmRSA.Encode(message, publicExponent, module);

            Console.WriteLine("Your crypted data:");
            foreach (var integer in encoded)
            {
                Console.WriteLine(integer.ToString());
            }
        }
    }
}
