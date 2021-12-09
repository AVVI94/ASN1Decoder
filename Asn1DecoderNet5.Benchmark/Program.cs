using System;
using Asn1DecoderNet5.Interfaces;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Asn1DecoderNet5.Benchmark
{
    class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Bench>();
        }
    }
}
