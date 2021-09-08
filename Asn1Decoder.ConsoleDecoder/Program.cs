using System;
using System.IO;
using Asn1DecoderNet5;

namespace Asn1Decoder.ConsoleDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Zadejte cestu k DER souboru pro dekodování:");
            var path = Console.ReadLine();
            if (path.StartsWith("\""))
                path = path.Substring(1, path.Length - 2);
            var bts = File.ReadAllBytes(path);
            try
            {
                Console.WriteLine(Decoder.TagToString(Decoder.Decode(bts), " | ", 128));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Console.Read();
        }
    }
}
