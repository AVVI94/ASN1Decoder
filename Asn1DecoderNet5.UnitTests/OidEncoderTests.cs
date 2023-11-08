using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Asn1DecoderNet5.UnitTests
{
    public class OidEncoderTests
    {
        [Theory]
        [InlineData("1.3.6.1.4.1.311.21.20", new byte[] { 0x2b, 0x06, 0x01, 0x04, 0x01, 0x82, 0x37, 0x15, 0x14 })]
        [InlineData("1.2.840.113549.1.1.11", new byte[] { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x0B, })]
        public void Should_FromString(string oid, byte[] expectedOutput)
        {
            var b = Asn1DecoderNet5.OIDEncoding.OidEncoding.GetBytes(oid);
            Assert.Equal(expectedOutput, b);
        }

        [Theory]
        [InlineData("1.3.6.1.4.1.311.21.20", new byte[] { 0x2b, 0x06, 0x01, 0x04, 0x01, 0x82, 0x37, 0x15, 0x14 })]
        [InlineData("1.2.840.113549.1.1.11", new byte[] { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x0B, })]
        public void Should_ToString(string expected, byte[] oid)
        {
            var b = Asn1DecoderNet5.OIDEncoding.OidEncoding.GetString(oid);
            Assert.Equal(expected, b);
        }
    }
}
