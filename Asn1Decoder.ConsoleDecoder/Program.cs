using System;
using System.IO;
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
