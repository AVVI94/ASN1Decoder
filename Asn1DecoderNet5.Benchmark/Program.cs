using System;
using Asn1Decoder.Interfaces;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Asn1Decoder.Benchmark
{
    class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Bench>();
        }
    }
}
