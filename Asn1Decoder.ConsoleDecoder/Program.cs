using System;
using System.IO;
using System.Linq;
using Asn1DecoderNet5;

namespace Asn1Decoder.ConsoleDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a DER file path:");
            var path = Console.ReadLine();
            if (path.StartsWith("\""))
                path = path[1..^1];
            var bts = path.EndsWith(".der") ? File.ReadAllBytes(path) : ConvertPemStringTyByteArray(File.ReadAllText(path));
            try
            {
                var tag = (Decoder.Decode(bts));
                var s =  Decoder.TagToString(tag, " | ", 128);
                Console.WriteLine(s);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Console.Read();
        }

        public static string[] PemStarts = {
            "-----BEGIN CERTIFICATE-----",
            "-----BEGIN CERTIFICATE REQUEST-----",
            "-----BEGIN PKCS7-----",
            "-----BEGIN PRIVATE KEY-----",
            "-----BEGIN ENCRYPTED PRIVATE KEY-----",
            "-----BEGIN RSA PRIVATE KEY-----"
        };

        public static string[] PemEnds = {
            "-----END CERTIFICATE-----",
            "-----END CERTIFICATE REQUEST-----",
            "-----END PKCS7-----",
            "-----END PRIVATE KEY-----",
            "-----END ENCRYPTED PRIVATE KEY-----",
            "-----END RSA PRIVATE KEY-----"
        };

        public static bool IsPemStringValid(string str)
        {
            str = str.Trim();
            if (PemStarts.Any(start => str.StartsWith(start)))
                if (PemEnds.Any(end => str.EndsWith(end)))
                    return true;
                else
                    return false;
            else
                return false;
        }

        public static byte[] ConvertPemStringTyByteArray(string pemString)
        {
            pemString = PreparePemStringForConversion(pemString);

            byte[] pemArr;

            try //pokus o převod PEM na byte[]
            {
                pemArr = Convert.FromBase64String(pemString);
            }
            catch
            {
                throw;
            }

            return pemArr;
        }

        public static string PreparePemStringForConversion(string PemString)
        {

            foreach (var s in PemStarts.Concat(PemEnds))
            {
                PemString = PemString.Replace(s, "");
            }
            return PemString.Replace("\r", "").Replace("\n", "");
        }
    }
}
