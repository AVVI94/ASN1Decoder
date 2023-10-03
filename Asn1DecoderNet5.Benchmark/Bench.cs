﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using BenchmarkDotNet.Attributes;

namespace Asn1DecoderNet5.Benchmark
{
    [MemoryDiagnoser]
    public class Bench
    {
        public Bench()
        {
            tag = Decode();
        }

        ITag tag;
        //[Benchmark]
        public ITag Decode() => Decoder.Decode(new byte[] { 0x30, 0x82, 0x07, 0xBF, 0x30, 0x82, 0x05, 0xA7, 0xA0, 0x03, 0x02, 0x01, 0x02, 0x02, 0x04, 0x00, 0xB5, 0x2D, 0x11, 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x0B, 0x05, 0x00, 0x30, 0x7F, 0x31, 0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x43, 0x5A, 0x31, 0x28, 0x30, 0x26, 0x06, 0x03, 0x55, 0x04, 0x03, 0x0C, 0x1F, 0x49, 0x2E, 0x43, 0x41, 0x20, 0x51, 0x75, 0x61, 0x6C, 0x69, 0x66, 0x69, 0x65, 0x64, 0x20, 0x32, 0x20, 0x43, 0x41, 0x2F, 0x52, 0x53, 0x41, 0x20, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36, 0x31, 0x2D, 0x30, 0x2B, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x0C, 0x24, 0x50, 0x72, 0x76, 0x6E, 0xC3, 0xAD, 0x20, 0x63, 0x65, 0x72, 0x74, 0x69, 0x66, 0x69, 0x6B, 0x61, 0xC4, 0x8D, 0x6E, 0xC3, 0xAD, 0x20, 0x61, 0x75, 0x74, 0x6F, 0x72, 0x69, 0x74, 0x61, 0x2C, 0x20, 0x61, 0x2E, 0x73, 0x2E, 0x31, 0x17, 0x30, 0x15, 0x06, 0x03, 0x55, 0x04, 0x05, 0x13, 0x0E, 0x4E, 0x54, 0x52, 0x43, 0x5A, 0x2D, 0x32, 0x36, 0x34, 0x33, 0x39, 0x33, 0x39, 0x35, 0x30, 0x1E, 0x17, 0x0D, 0x32, 0x31, 0x30, 0x38, 0x32, 0x35, 0x31, 0x31, 0x33, 0x36, 0x35, 0x30, 0x5A, 0x17, 0x0D, 0x32, 0x32, 0x30, 0x38, 0x32, 0x35, 0x31, 0x31, 0x33, 0x36, 0x35, 0x30, 0x5A, 0x30, 0x5E, 0x31, 0x15, 0x30, 0x13, 0x06, 0x03, 0x55, 0x04, 0x03, 0x0C, 0x0C, 0x52, 0x6F, 0x6D, 0x61, 0x6E, 0x20, 0x4B, 0x6F, 0xC4, 0x8D, 0xC3, 0xAD, 0x31, 0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x43, 0x5A, 0x31, 0x0E, 0x30, 0x0C, 0x06, 0x03, 0x55, 0x04, 0x2A, 0x0C, 0x05, 0x52, 0x6F, 0x6D, 0x61, 0x6E, 0x31, 0x0F, 0x30, 0x0D, 0x06, 0x03, 0x55, 0x04, 0x04, 0x0C, 0x06, 0x4B, 0x6F, 0xC4, 0x8D, 0xC3, 0xAD, 0x31, 0x17, 0x30, 0x15, 0x06, 0x03, 0x55, 0x04, 0x05, 0x13, 0x0E, 0x49, 0x43, 0x41, 0x20, 0x2D, 0x20, 0x31, 0x30, 0x36, 0x30, 0x35, 0x34, 0x35, 0x37, 0x30, 0x82, 0x01, 0x22, 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00, 0x03, 0x82, 0x01, 0x0F, 0x00, 0x30, 0x82, 0x01, 0x0A, 0x02, 0x82, 0x01, 0x01, 0x00, 0xD3, 0x56, 0x19, 0x3C, 0xA5, 0x28, 0x95, 0xC2, 0xD8, 0xA6, 0x9F, 0x25, 0x19, 0x2A, 0xF7, 0xC8, 0x39, 0xB7, 0xB0, 0xA4, 0xFB, 0xEF, 0xB6, 0xA2, 0xF1, 0x7E, 0x09, 0xE4, 0xD1, 0x7B, 0xC3, 0x5E, 0x11, 0xAD, 0x1F, 0xDE, 0x6A, 0x98, 0x82, 0xAC, 0x50, 0xA9, 0xE3, 0x88, 0x63, 0xF7, 0x34, 0xF3, 0x45, 0xD1, 0xE3, 0x6C, 0x38, 0xC8, 0x26, 0xFB, 0x17, 0x76, 0xEA, 0x43, 0xA1, 0x3D, 0x49, 0x6D, 0x08, 0xD3, 0x88, 0x50, 0xDE, 0xDB, 0xB1, 0x94, 0x79, 0xF0, 0xC0, 0x44, 0x49, 0x1A, 0x37, 0xBD, 0x95, 0x14, 0x01, 0x66, 0x70, 0x6C, 0xD5, 0xE1, 0x7C, 0xC5, 0x95, 0x60, 0x58, 0x25, 0xB8, 0xB9, 0x47, 0x07, 0x76, 0x7E, 0xFC, 0x1D, 0x90, 0x0F, 0x40, 0x4C, 0x99, 0x71, 0xE2, 0x32, 0xC1, 0xE7, 0x5C, 0x24, 0x25, 0x33, 0x55, 0xB6, 0x26, 0x51, 0x93, 0x13, 0xB2, 0x7E, 0xFC, 0x6F, 0xA8, 0x91, 0xD6, 0x01, 0xA6, 0xE2, 0x54, 0xEA, 0x3D, 0xBE, 0x0E, 0x88, 0x8A, 0xF5, 0xEB, 0xCC, 0xD3, 0x8F, 0xF5, 0xE1, 0x89, 0x8F, 0x5A, 0x91, 0xAA, 0x2F, 0x11, 0x5E, 0x62, 0xC8, 0x7B, 0x5D, 0xE5, 0xB5, 0xD7, 0x3C, 0x2D, 0xEE, 0xE6, 0x2F, 0x4A, 0x88, 0x88, 0x3B, 0x09, 0x43, 0x56, 0x85, 0xEA, 0xD9, 0x99, 0x9D, 0xF8, 0x10, 0x97, 0x97, 0x44, 0xBB, 0xCE, 0x98, 0xA3, 0x0E, 0xA6, 0x62, 0x6D, 0x77, 0xE2, 0x57, 0x21, 0x32, 0x48, 0xCA, 0xE1, 0xFD, 0x0A, 0xFC, 0xA4, 0xB7, 0xED, 0x9A, 0x15, 0x7D, 0xE2, 0x9B, 0x6F, 0x36, 0xB2, 0xA7, 0x64, 0xA6, 0x9F, 0x07, 0xB5, 0x55, 0x26, 0x74, 0x88, 0x7D, 0x60, 0xC2, 0x6A, 0x6F, 0x16, 0xE5, 0x2B, 0x8A, 0x60, 0xAD, 0xB2, 0xC6, 0x1A, 0x9E, 0x6A, 0x75, 0xAA, 0xA5, 0xF4, 0xF6, 0xE8, 0xDA, 0xE3, 0xE1, 0xDE, 0xBE, 0x81, 0x35, 0x8E, 0x7F, 0x3F, 0x29, 0x02, 0x03, 0x01, 0x00, 0x01, 0xA3, 0x82, 0x03, 0x62, 0x30, 0x82, 0x03, 0x5E, 0x30, 0x3E, 0x06, 0x03, 0x55, 0x1D, 0x11, 0x04, 0x37, 0x30, 0x35, 0xA0, 0x18, 0x06, 0x0A, 0x2B, 0x06, 0x01, 0x04, 0x01, 0x81, 0xB8, 0x48, 0x04, 0x06, 0xA0, 0x0A, 0x0C, 0x08, 0x31, 0x30, 0x36, 0x30, 0x35, 0x34, 0x35, 0x37, 0xA0, 0x19, 0x06, 0x09, 0x2B, 0x06, 0x01, 0x04, 0x01, 0xDC, 0x19, 0x02, 0x01, 0xA0, 0x0C, 0x0C, 0x0A, 0x31, 0x38, 0x37, 0x38, 0x33, 0x32, 0x32, 0x33, 0x38, 0x30, 0x30, 0x0E, 0x06, 0x03, 0x55, 0x1D, 0x0F, 0x01, 0x01, 0xFF, 0x04, 0x04, 0x03, 0x02, 0x06, 0xC0, 0x30, 0x09, 0x06, 0x03, 0x55, 0x1D, 0x13, 0x04, 0x02, 0x30, 0x00, 0x30, 0x82, 0x01, 0x28, 0x06, 0x03, 0x55, 0x1D, 0x20, 0x04, 0x82, 0x01, 0x1F, 0x30, 0x82, 0x01, 0x1B, 0x30, 0x82, 0x01, 0x0C, 0x06, 0x0D, 0x2B, 0x06, 0x01, 0x04, 0x01, 0x81, 0xB8, 0x48, 0x0A, 0x01, 0x1E, 0x01, 0x01, 0x30, 0x81, 0xFA, 0x30, 0x1D, 0x06, 0x08, 0x2B, 0x06, 0x01, 0x05, 0x05, 0x07, 0x02, 0x01, 0x16, 0x11, 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x77, 0x77, 0x77, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x30, 0x81, 0xD8, 0x06, 0x08, 0x2B, 0x06, 0x01, 0x05, 0x05, 0x07, 0x02, 0x02, 0x30, 0x81, 0xCB, 0x0C, 0x81, 0xC8, 0x54, 0x65, 0x6E, 0x74, 0x6F, 0x20, 0x6B, 0x76, 0x61, 0x6C, 0x69, 0x66, 0x69, 0x6B, 0x6F, 0x76, 0x61, 0x6E, 0x79, 0x20, 0x63, 0x65, 0x72, 0x74, 0x69, 0x66, 0x69, 0x6B, 0x61, 0x74, 0x20, 0x70, 0x72, 0x6F, 0x20, 0x65, 0x6C, 0x65, 0x6B, 0x74, 0x72, 0x6F, 0x6E, 0x69, 0x63, 0x6B, 0x79, 0x20, 0x70, 0x6F, 0x64, 0x70, 0x69, 0x73, 0x20, 0x62, 0x79, 0x6C, 0x20, 0x76, 0x79, 0x64, 0x61, 0x6E, 0x20, 0x76, 0x20, 0x73, 0x6F, 0x75, 0x6C, 0x61, 0x64, 0x75, 0x20, 0x73, 0x20, 0x6E, 0x61, 0x72, 0x69, 0x7A, 0x65, 0x6E, 0x69, 0x6D, 0x20, 0x45, 0x55, 0x20, 0x63, 0x2E, 0x20, 0x39, 0x31, 0x30, 0x2F, 0x32, 0x30, 0x31, 0x34, 0x2E, 0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x71, 0x75, 0x61, 0x6C, 0x69, 0x66, 0x69, 0x65, 0x64, 0x20, 0x63, 0x65, 0x72, 0x74, 0x69, 0x66, 0x69, 0x63, 0x61, 0x74, 0x65, 0x20, 0x66, 0x6F, 0x72, 0x20, 0x65, 0x6C, 0x65, 0x63, 0x74, 0x72, 0x6F, 0x6E, 0x69, 0x63, 0x20, 0x73, 0x69, 0x67, 0x6E, 0x61, 0x74, 0x75, 0x72, 0x65, 0x20, 0x61, 0x63, 0x63, 0x6F, 0x72, 0x64, 0x69, 0x6E, 0x67, 0x20, 0x74, 0x6F, 0x20, 0x52, 0x65, 0x67, 0x75, 0x6C, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x28, 0x45, 0x55, 0x29, 0x20, 0x4E, 0x6F, 0x20, 0x39, 0x31, 0x30, 0x2F, 0x32, 0x30, 0x31, 0x34, 0x2E, 0x30, 0x09, 0x06, 0x07, 0x04, 0x00, 0x8B, 0xEC, 0x40, 0x01, 0x00, 0x30, 0x81, 0x8F, 0x06, 0x03, 0x55, 0x1D, 0x1F, 0x04, 0x81, 0x87, 0x30, 0x81, 0x84, 0x30, 0x2A, 0xA0, 0x28, 0xA0, 0x26, 0x86, 0x24, 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x71, 0x63, 0x72, 0x6C, 0x64, 0x70, 0x31, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x2F, 0x32, 0x71, 0x63, 0x61, 0x31, 0x36, 0x5F, 0x72, 0x73, 0x61, 0x2E, 0x63, 0x72, 0x6C, 0x30, 0x2A, 0xA0, 0x28, 0xA0, 0x26, 0x86, 0x24, 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x71, 0x63, 0x72, 0x6C, 0x64, 0x70, 0x32, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x2F, 0x32, 0x71, 0x63, 0x61, 0x31, 0x36, 0x5F, 0x72, 0x73, 0x61, 0x2E, 0x63, 0x72, 0x6C, 0x30, 0x2A, 0xA0, 0x28, 0xA0, 0x26, 0x86, 0x24, 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x71, 0x63, 0x72, 0x6C, 0x64, 0x70, 0x33, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x2F, 0x32, 0x71, 0x63, 0x61, 0x31, 0x36, 0x5F, 0x72, 0x73, 0x61, 0x2E, 0x63, 0x72, 0x6C, 0x30, 0x81, 0x86, 0x06, 0x08, 0x2B, 0x06, 0x01, 0x05, 0x05, 0x07, 0x01, 0x03, 0x04, 0x7A, 0x30, 0x78, 0x30, 0x08, 0x06, 0x06, 0x04, 0x00, 0x8E, 0x46, 0x01, 0x01, 0x30, 0x57, 0x06, 0x06, 0x04, 0x00, 0x8E, 0x46, 0x01, 0x05, 0x30, 0x4D, 0x30, 0x2D, 0x16, 0x27, 0x68, 0x74, 0x74, 0x70, 0x73, 0x3A, 0x2F, 0x2F, 0x77, 0x77, 0x77, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x2F, 0x5A, 0x70, 0x72, 0x61, 0x76, 0x79, 0x2D, 0x70, 0x72, 0x6F, 0x2D, 0x75, 0x7A, 0x69, 0x76, 0x61, 0x74, 0x65, 0x6C, 0x65, 0x13, 0x02, 0x63, 0x73, 0x30, 0x1C, 0x16, 0x16, 0x68, 0x74, 0x74, 0x70, 0x73, 0x3A, 0x2F, 0x2F, 0x77, 0x77, 0x77, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x2F, 0x50, 0x44, 0x53, 0x13, 0x02, 0x65, 0x6E, 0x30, 0x13, 0x06, 0x06, 0x04, 0x00, 0x8E, 0x46, 0x01, 0x06, 0x30, 0x09, 0x06, 0x07, 0x04, 0x00, 0x8E, 0x46, 0x01, 0x06, 0x01, 0x30, 0x65, 0x06, 0x08, 0x2B, 0x06, 0x01, 0x05, 0x05, 0x07, 0x01, 0x01, 0x04, 0x59, 0x30, 0x57, 0x30, 0x2A, 0x06, 0x08, 0x2B, 0x06, 0x01, 0x05, 0x05, 0x07, 0x30, 0x02, 0x86, 0x1E, 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x71, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x2F, 0x32, 0x71, 0x63, 0x61, 0x31, 0x36, 0x5F, 0x72, 0x73, 0x61, 0x2E, 0x63, 0x65, 0x72, 0x30, 0x29, 0x06, 0x08, 0x2B, 0x06, 0x01, 0x05, 0x05, 0x07, 0x30, 0x01, 0x86, 0x1D, 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x6F, 0x63, 0x73, 0x70, 0x2E, 0x69, 0x63, 0x61, 0x2E, 0x63, 0x7A, 0x2F, 0x32, 0x71, 0x63, 0x61, 0x31, 0x36, 0x5F, 0x72, 0x73, 0x61, 0x30, 0x1F, 0x06, 0x03, 0x55, 0x1D, 0x23, 0x04, 0x18, 0x30, 0x16, 0x80, 0x14, 0x74, 0x82, 0x08, 0x91, 0xE3, 0xD9, 0x64, 0x68, 0x71, 0x85, 0xD6, 0xEB, 0x31, 0xE4, 0x72, 0xDF, 0x8B, 0x26, 0xB1, 0x6D, 0x30, 0x1D, 0x06, 0x03, 0x55, 0x1D, 0x0E, 0x04, 0x16, 0x04, 0x14, 0xA8, 0x4A, 0xE6, 0x09, 0x7D, 0x7A, 0x00, 0x9A, 0x81, 0xE0, 0x6F, 0x1B, 0x4E, 0x51, 0x75, 0x04, 0x6E, 0x7A, 0xDF, 0x90, 0x30, 0x13, 0x06, 0x03, 0x55, 0x1D, 0x25, 0x04, 0x0C, 0x30, 0x0A, 0x06, 0x08, 0x2B, 0x06, 0x01, 0x05, 0x05, 0x07, 0x03, 0x04, 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x0B, 0x05, 0x00, 0x03, 0x82, 0x02, 0x01, 0x00, 0x2B, 0x00, 0x29, 0xDD, 0xE9, 0x80, 0x5B, 0x1B, 0x1C, 0x9B, 0xCB, 0x15, 0xFF, 0x49, 0x5B, 0xA5, 0x02, 0xBB, 0x0F, 0xE5, 0x93, 0xE2, 0x2A, 0xF5, 0x24, 0x84, 0xF0, 0x4C, 0xC5, 0x61, 0x4C, 0x97, 0x6F, 0x67, 0x5B, 0xA3, 0xDE, 0x45, 0x83, 0x48, 0x5A, 0x5A, 0xB3, 0x4D, 0x88, 0x91, 0x08, 0xF3, 0xD2, 0x33, 0x11, 0x4F, 0x85, 0x49, 0x2F, 0x4A, 0x34, 0xE0, 0x59, 0x6C, 0x3B, 0x17, 0x82, 0x5B, 0xE8, 0xDF, 0x4A, 0x29, 0x2B, 0xB0, 0xD3, 0x70, 0x85, 0x29, 0xE7, 0x95, 0xFA, 0xF5, 0x2F, 0xA2, 0x62, 0xD2, 0x72, 0x54, 0x0F, 0x39, 0x45, 0xD1, 0x2C, 0xDC, 0x5E, 0xC2, 0xDC, 0x39, 0x34, 0xDA, 0x93, 0x3E, 0x8A, 0xE5, 0xD8, 0xC3, 0xC5, 0x38, 0x81, 0x9D, 0x49, 0x18, 0x4F, 0x72, 0xC8, 0xEE, 0x11, 0x16, 0x1F, 0x2D, 0xEA, 0xEE, 0x50, 0x5D, 0xBA, 0x02, 0xC0, 0xF3, 0x9A, 0x34, 0xE8, 0x70, 0x62, 0xE9, 0x0A, 0xAF, 0x56, 0xA3, 0x92, 0xE5, 0x10, 0xA4, 0x5C, 0xCF, 0x50, 0x90, 0xF5, 0x1F, 0xD4, 0x8F, 0x01, 0x89, 0xDE, 0x3C, 0xA9, 0x78, 0xEE, 0x80, 0xFD, 0x1A, 0xC2, 0x2E, 0x83, 0xC5, 0x02, 0xD2, 0xBC, 0xA8, 0x9C, 0x30, 0x7E, 0x12, 0xF1, 0xAA, 0xF2, 0xAD, 0xFD, 0x03, 0xC4, 0xFA, 0x4B, 0x92, 0x96, 0x2C, 0x98, 0xE0, 0x8A, 0xDF, 0xD2, 0xD3, 0x10, 0x8D, 0x87, 0x1E, 0x19, 0x09, 0x6C, 0x02, 0x53, 0x1A, 0xF0, 0xDE, 0x16, 0x30, 0xCF, 0x5A, 0xD7, 0x2D, 0x58, 0x6D, 0x99, 0x92, 0x3B, 0xA3, 0xDE, 0xCE, 0x53, 0xA3, 0x4F, 0x28, 0x77, 0x0D, 0x7F, 0x55, 0x07, 0xA5, 0x7B, 0x20, 0x62, 0xBC, 0xB4, 0x0E, 0x08, 0x93, 0x52, 0x6E, 0x4E, 0xEE, 0xA9, 0x63, 0xCF, 0x3D, 0x72, 0xE4, 0x37, 0x37, 0xED, 0x3C, 0x4F, 0x1C, 0x3B, 0x92, 0x60, 0x87, 0xB0, 0x96, 0x7E, 0xF6, 0x80, 0x36, 0xD9, 0x8F, 0x88, 0x20, 0x01, 0xC6, 0x33, 0xD9, 0x17, 0xB8, 0x86, 0x1B, 0x53, 0xF0, 0x96, 0x69, 0xD0, 0x5E, 0xD8, 0x4E, 0x87, 0xB1, 0x41, 0xFD, 0x2B, 0xEB, 0x36, 0x80, 0xD0, 0x7C, 0x98, 0xB3, 0x77, 0x9F, 0xD1, 0x1C, 0x3D, 0x88, 0x20, 0x67, 0x2E, 0xE4, 0xF2, 0x84, 0x94, 0x1C, 0x49, 0x35, 0x9A, 0x34, 0xF0, 0x43, 0xDA, 0x79, 0x32, 0x3D, 0x31, 0xDC, 0x3D, 0x97, 0x99, 0xC1, 0xA4, 0x05, 0x35, 0x1E, 0x8A, 0xDF, 0x24, 0x4B, 0xDC, 0x69, 0xBC, 0x1A, 0x5F, 0x48, 0x1E, 0x08, 0x29, 0xFE, 0x4D, 0x4C, 0x0B, 0x04, 0xBD, 0x72, 0x4D, 0xAD, 0x8A, 0xE3, 0xEE, 0x7F, 0x85, 0x09, 0xAD, 0x00, 0xC2, 0x54, 0xF1, 0x85, 0xA8, 0x4B, 0xCC, 0x01, 0x33, 0xEE, 0x03, 0x98, 0x2D, 0x5A, 0x1E, 0x74, 0x9D, 0x52, 0xE0, 0xCE, 0xE7, 0xC3, 0x3E, 0xE0, 0xAD, 0x19, 0x4E, 0xF3, 0x03, 0xD3, 0x1C, 0xAE, 0x3E, 0xB3, 0x04, 0x00, 0x80, 0x3A, 0x23, 0x32, 0xE0, 0x94, 0xCF, 0x5C, 0x0C, 0x85, 0xB3, 0x35, 0x28, 0x42, 0x14, 0x50, 0xE0, 0xF0, 0x69, 0x6F, 0xA4, 0xA1, 0x49, 0x76, 0x07, 0x7D, 0xB3, 0xD1, 0x89, 0x1F, 0x4C, 0x41, 0x81, 0xA6, 0xFE, 0x04, 0x06, 0xBC, 0x90, 0x8D, 0xED, 0x59, 0x9C, 0xB3, 0xE2, 0x27, 0x60, 0x93, 0x3D, 0xE4, 0xAD, 0x47, 0x12, 0xE8, 0x0D, 0x80, 0x85, 0x63, 0xF3, 0x75, 0xCC, 0x8A, 0x46, 0x6C, 0x99, 0x57, 0x5D, 0x01, 0xF1, 0xA0, 0xF6, 0x06, 0x58, 0x5F, 0x51, 0xD0, 0x2D, 0x0F, 0xA1, 0x7F, 0xFC, 0x90, 0xFC, 0xDF, 0x2F, 0xE6, 0xEC, 0x75, 0xF4, 0x8C, 0xED, 0x79, 0xEA, 0x6B, 0x65, 0x71, 0xF8, 0x5D, 0xBB, 0x7F, 0xBF, 0xDE, 0x8E, 0x4F, 0x3F, 0xB6, 0xEE, 0xC3, 0xE6, 0xC8, 0x39, 0x19, 0x1C, 0x23, 0xA9, 0x29, 0x56, 0x58, 0xFB, 0xD9, 0x55, 0xAD, 0x5D, 0xE2 });
        //[Benchmark]
        public void TagToString() => Decoder.TagToString(tag, "", 128);

        //[Benchmark]
        public byte[] OidToBytes() => Encoding.OidEncoding.GetBytes("1.2.840.113549.1.1.11");

        [Benchmark]
        public bool IsCert() => tag.IsCertificate();
    }
}
