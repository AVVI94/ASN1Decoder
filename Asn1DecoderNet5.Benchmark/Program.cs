using System;
using ASN1Decoder.NET.Interfaces;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ASN1Decoder.NET.Benchmark
{
    class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Bench>();
        }
    }
}
