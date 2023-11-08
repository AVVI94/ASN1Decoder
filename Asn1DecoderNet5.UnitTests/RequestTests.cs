using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.CSR;
using Xunit;

namespace Asn1DecoderNet5.UnitTests;
public class RequestTests
{
    [Fact]
    public void Should_CreateRequest()
    {
        var r = new CertificationRequest(OID.GetOrCreate("1.2.840.113549.1.1.11"), SHA256.Create());
        var kp = new RSACryptoServiceProvider(2048);
        r.AddSubjectItem(SubjectItemKind.CommonName, Encoding.UTF8.GetBytes("Test Test"), true);
        r.AddSubjectItem(SubjectItemKind.Surname, Encoding.UTF8.GetBytes("Test"), true);
        r.AddSubjectItem(SubjectItemKind.GivenName, Encoding.UTF8.GetBytes("Test"), true);
        r.AddSubjectItem(SubjectItemKind.CountryName, Encoding.UTF8.GetBytes("CZ"), false);
        r.SetKeyUsage(new Tags.KeyUsage(true, true, true, true));
        r.AddExtensionRequest(
            new Tags.ObjectIdentifier(OID.GetOrCreate(OID.SUBJECT_ALT_NAME).ByteValue),
            false,
            new Tags.OctetString(children: new List<Interfaces.IReadOnlyTag>()
            {
                new Tags.Sequence(new List<Interfaces.IReadOnlyTag>()
                {
                    new Tags.ContextSpecific_1(Encoding.ASCII.GetBytes("test@test.cz"))
                })
            }));
        var req = r.BuildRequest(kp);
    }
}
