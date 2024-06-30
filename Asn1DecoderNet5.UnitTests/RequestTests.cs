using ASN1Decoder.NET.CSR;
using ASN1Decoder.NET.Tags;
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace ASN1Decoder.NET.UnitTests;
public class RequestTests
{
    //[Fact]
    //public void Should_CreateRequest()
    //{
    //    var r = new RSACertificationRequestBuilder(OID.GetOrCreate("1.2.840.113549.1.1.11"), SHA256.Create());
    //    var kp = new RSACryptoServiceProvider(2048);
    //    var ecc = new ECDsaCng(ECCurve.NamedCurves.nistP384);
    //    ecc.SignData([], HashAlgorithmName.SHA256);
    //    r.AddSubjectItem(SubjectItemKind.CommonName, Encoding.UTF8.GetBytes("Test Test"), true);
    //    r.AddSubjectItem(SubjectItemKind.Surname, Encoding.UTF8.GetBytes("Test"), true);
    //    r.AddSubjectItem(SubjectItemKind.GivenName, Encoding.UTF8.GetBytes("Test"), true);
    //    r.AddSubjectItem(SubjectItemKind.CountryName, Encoding.UTF8.GetBytes("CZ"), false);
    //    r.SetKeyUsage(new Tags.KeyUsage(true, true, true, true));
    //    r.AddExtensionRequest(
    //        new Tags.ObjectIdentifier(OID.GetOrCreate(OID.SUBJECT_ALT_NAME).ByteValue),
    //        false,
    //        new Tags.OctetString(children: new List<Interfaces.IReadOnlyTag>()
    //        {
    //            new Tags.Sequence(new List<Interfaces.IReadOnlyTag>()
    //            {
    //                new Tags.ContextSpecific_1(Encoding.ASCII.GetBytes("test@test.cz"))
    //            })
    //        }));
    //    var req = r.BuildRequest(kp);
    //    var base64 = Convert.ToBase64String(req);
    //}

    [Fact]
    public void Should_CreateRequestWithRSAKey()
    {
        var builder = new RequestBuilder();
        builder.WithSubject(x =>
        {
            x.CommonName("Test Test")
             .CountryName("CZ")
             .GivenName("Test")
             .Surname("Test");
        })
            .SetKeyUsage(new KeyUsage(true, true, true, true))
            .WithExtendedKeyUsage("1.3.6.1.5.5.7.3.2")
            .WithExtensionRequest(new ObjectIdentifier(OID.GetOrCreate(OID.SUBJECT_ALT_NAME).ByteValue),
            false,
            new OctetString(null, [
                new Sequence([
                    new ContextSpecific_1(null, [
                        new OctetString(System.Text.Encoding.UTF8.GetBytes("test@test.cz"))
                        ])
                    ])
                ])
            );
        var key = RSA.Create(2048);
        var req = builder.BuildCSR(key, OID.GetOrCreate("1.2.840.113549.1.1.11"), HashAlgorithmName.SHA256);
        var base64 = Convert.ToBase64String(req);
    }
    [Fact]
    public void Should_CreateRequestWithECDsaKey()
    {
        var builder = new RequestBuilder();
        builder.WithSubject(x =>
        {
            x.CommonName("Test Test")
             .CountryName("CZ")
             .GivenName("Test")
             .Surname("Test");
        })
            .SetKeyUsage(new KeyUsage(true, true, true, true))
            .WithExtendedKeyUsage("1.3.6.1.5.5.7.3.2")
            .WithExtensionRequest(new ObjectIdentifier(OID.GetOrCreate(OID.SUBJECT_ALT_NAME).ByteValue),
            false,
            new OctetString(null, [
                new Sequence([
                    new ContextSpecific_1(null, [
                        new OctetString(System.Text.Encoding.UTF8.GetBytes("test@test.cz"))
                        ])
                    ])
                ])
            );
        var key = ECDsa.Create(ECCurve.NamedCurves.nistP384);
        var req = builder.BuildCSR(key, OID.GetOrCreate("1.2.840.10045.4.3.3"), HashAlgorithmName.SHA384);
        var base64 = Convert.ToBase64String(req);
    }
}
