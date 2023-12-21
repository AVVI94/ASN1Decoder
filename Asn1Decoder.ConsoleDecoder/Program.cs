using System;
using System.IO;
using System.Linq;
using Asn1Decoder;

namespace Asn1Decoder.ConsoleDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a DER file path:");
            var path = Console.ReadLine();
            if (path.StartsWith("\""))
                path = path.Remove(0, 1).Remove(path.Length - 2, 1);
            var bts = path.EndsWith(".der") ? File.ReadAllBytes(path) : ConvertPemStringTyByteArray(File.ReadAllText(path));
            Console.WriteLine("Should output be desctructurized? Yes - No [Y/N]");
            var wantDestructurizedOutput = Console.ReadKey(true).KeyChar.ToString().ToLower() == "y";
            try
            {
                var tag = (Decoder.Decode(bts));
                string s;
                if (!wantDestructurizedOutput)
                    s = Decoder.TagToString(tag, " | ", 128);
                else
                {
                    var list = Decoder.Desctructurize(tag);
                    list.ForEach(x => x.ConvertContentToReadableContent());
                    s = string.Join(Environment.NewLine, list.Select(x=>$"{x.TagName} {x.ReadableContent}"));
                }
                Console.WriteLine(s);
            }
            catch (Exception e)
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
