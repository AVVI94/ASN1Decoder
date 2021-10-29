using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Asn1DecoderNet5.UnitTests
{
    public class DecoderTests
    {
        [Fact]
        public void DecodeNormalCertificate()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var decodedString = Decoder.TagToString(tag, " | ", 128);

            Assert.Equal(Results.NormalCertificate, decodedString);
        }
        [Fact]
        public void DecodeNormalCertificate_NoSeparator()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var decodedString = Decoder.TagToString(tag, "", 128);

            Assert.Equal(Results.NormalCertificate_NoSeparator, decodedString);
        }
        [Fact]
        public void DecodeNormalCertificate_NoLineLength()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var decodedString = Decoder.TagToString(tag, " | ", 0);

            Assert.Equal(Results.NormalCertificate_NoLineLength, decodedString);
        }
        [Fact]
        public void DecodeNormalCertificate_NoLineLength_NoSeparator()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var decodedString = Decoder.TagToString(tag, "", 0);

            Assert.Equal(Results.NormalCertificate_NoLineLength_NoSeparator, decodedString);
        }
        [Fact]
        public void DecodeNormalCertificate_NegativeLength()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var decodedString = Decoder.TagToString(tag, " | ", -1);

            Assert.Equal(Results.NormalCertificate_NoLineLength, decodedString);
        }

        [Fact]
        public void DecodeUndefinedLengthPfx()
        {
            var tag = Decoder.Decode(Sources.UndefinedLengthPfx);
            var decodedString = Decoder.TagToString(tag, " | ", 128);

            Assert.Equal(Results.UndefinedLengthPfx, decodedString);
        }
        [Fact]
        public void DecodeUndefinedLengthPfx_NoSeparator()
        {
            var tag = Decoder.Decode(Sources.UndefinedLengthPfx);
            var decodedString = Decoder.TagToString(tag, "", 128);

            Assert.Equal(Results.UndefinedLengthPfx_NoSeparator, decodedString);
        }
        [Fact]
        public void DecodeUndefinedLengthPfx_NoLineLength()
        {
            var tag = Decoder.Decode(Sources.UndefinedLengthPfx);
            var decodedString = Decoder.TagToString(tag, " | ", 0);

            Assert.Equal(Results.UndefinedLengthPfx_NoLineLength, decodedString);
        }
        [Fact]
        public void DecodeUndefinedLengthPfx_NoLineLength_NoSeparator()
        {
            var tag = Decoder.Decode(Sources.UndefinedLengthPfx);
            var decodedString = Decoder.TagToString(tag, "", 0);

            Assert.Equal(Results.UndefinedLengthPfx_NoLineLength_NoSeparator, decodedString);
        }
        [Fact]
        public void DecodeUndefinedLengthPfx_NegativeLineLength()
        {
            var tag = Decoder.Decode(Sources.UndefinedLengthPfx);
            var decodedString = Decoder.TagToString(tag, " | ", -1);

            Assert.Equal(Results.UndefinedLengthPfx_NoLineLength, decodedString);
        }

    }
}
