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
        public void GetCommonName_Issuer()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var ok = tag.TryGetCertificateSubjectItem(SubjectItemKind.CommonName, true, out var res);
            Assert.True(ok);
            Assert.Equal("I.CA Test Qualified 2 CA/ECC 06/2019", res[0]);
        }
        
        [Fact]
        public void Should_IsCertificate_ReturnsTrue()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var ok = tag.IsCertificate();
            Assert.True(ok);
        }
        [Fact]
        public void GetCommonName_Subject()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var ok = tag.TryGetCertificateSubjectItem(SubjectItemKind.CommonName, false, out var res);
            Assert.True(ok);
            Assert.Equal("EccTest CertRenewer", res[0]);
        }
        [Fact]
        public void GetCommonName()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var ok = tag.TryGetSubjectItem(Encoding.OidEncoding.GetBytes(OID.COMMON_NAME), out var items);
            Assert.True(ok);
            Assert.NotEmpty(items);
        }
        [Fact]
        public void GetKeyUsage()
        {
            var tag = Decoder.Decode(Sources.NormalCertificate);
            var ku = tag.TryGetKeyUsage(out var keyUsage);
            Assert.True(ku);
            Assert.False(keyUsage.KeyUsages == null);
        }
        
        [Fact]
        public void ShouldNotGetKeyUsage()
        {
            var tag = Decoder.Decode(Sources.UndefinedLengthPfx);
            var ku = tag.TryGetKeyUsage(out var keyUsage);
            Assert.False(ku);
            Assert.True(keyUsage.KeyUsages == null);
        }

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
