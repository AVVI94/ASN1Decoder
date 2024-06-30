using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ASN1Decoder.NET.Tags;
namespace ASN1Decoder.NET.CSR;

#nullable enable
public class RSACertificationRequestBuilder
{

    public RSACertificationRequestBuilder(OID signatureAlgorithmOid, HashAlgorithm signatureHashAlgorithm)
    {
        PublicKeyInfo = new Sequence(new List<ITag>()
        {
            new Sequence(new List<ITag>()
            {
                new ObjectIdentifier(OID.GetOrCreate(OID.RSA_ENCRYPTION).ByteValue),
                new Null()
            })
        });
        SignatureAlgorithmOid = signatureAlgorithmOid;
        SignatureHashAlgorithm = signatureHashAlgorithm;
    }

    public List<Set> Subject { get; } = new();
    public List<ITag> RequestedExtensions { get; } = new();
    public List<ITag> OtherElementsAfterExtensionRequestSequence { get; } = new();
    public OID SignatureAlgorithmOid { get; private set; }
    public HashAlgorithm SignatureHashAlgorithm { get; private set; }
    public Sequence PublicKeyInfo { get; private set; }
    //public AsymmetricAlgorithm? KeyPair { get; set; }

    public byte[] BuildRequest(RSACryptoServiceProvider keyPair)
    {
        var version = new Integer(new byte[1] { 0x0 });
        var subject = new Sequence(Subject.Cast<ITag>().ToList());
        var keyAlg = new Sequence(new List<ITag>()
        {
            new ObjectIdentifier(SignatureAlgorithmOid.ByteValue),
            new Null()
        });

        var keyParams = keyPair.ExportParameters(false);

        var key = new Sequence(new List<ITag>()
        {
            new Integer(keyParams.Modulus),
            new Integer(keyParams.Exponent),
        });
        var bkey = new List<byte>() { 0x0, 0x0 };
        bkey.AddRange(Decoder.Encode(key));

        ((List<ITag>)PublicKeyInfo.Children).Add(new BitString(bkey.ToArray()));

        var ext = new ContextSpecific_0(null, new List<ITag>()
        {
            new ObjectIdentifier(OID.OidDictionary[OID.EXTENSION_REQUEST].ByteValue),
            new Set(new List<ITag>()
            {
                new Sequence(RequestedExtensions)
            })
        });
        var certRequestInfo = new Sequence(new List<ITag>()
            {
                version,
                subject,
                PublicKeyInfo,
                ext
            });
        var toBeSigned = Decoder.Encode(certRequestInfo);
        var signature = keyPair.SignData(toBeSigned, SignatureHashAlgorithm);
        var request = new Sequence(new List<ITag>()
        {
            certRequestInfo,
            keyAlg,
            new BitString(signature)
        });
        return Decoder.Encode(request);
    }

    public void AddSubjectItem(SubjectItemKind itemKind, byte[] value, bool useUtf8StringValueTag)
    {
        static Sequence CreateSeq(ITag oid, ITag value) => new(new List<ITag>() { oid, value });
        static ObjectIdentifier CreateOid(SubjectItemKind itemKind) => new(OID.GetOrCreate(itemKind.ToOidString()).ByteValue);
        static ITag CreateVal(byte[] val, bool utf8) => utf8 ? new Utf8String(val) : new PrintableString(val);

        var seq = itemKind switch
        {
            SubjectItemKind.CommonName => CreateSeq(CreateOid(SubjectItemKind.CommonName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.GivenName => CreateSeq(CreateOid(SubjectItemKind.GivenName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.Surname => CreateSeq(CreateOid(SubjectItemKind.Surname), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.CountryName => CreateSeq(CreateOid(SubjectItemKind.CountryName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.OrganizationName => CreateSeq(CreateOid(SubjectItemKind.OrganizationName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.OrganizationUnitName => CreateSeq(CreateOid(SubjectItemKind.OrganizationUnitName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.StateOrProvinceName => CreateSeq(CreateOid(SubjectItemKind.StateOrProvinceName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.LocalityName => CreateSeq(CreateOid(SubjectItemKind.LocalityName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.SerialNumber => CreateSeq(CreateOid(SubjectItemKind.SerialNumber), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.Title => CreateSeq(CreateOid(SubjectItemKind.Title), CreateVal(value, useUtf8StringValueTag)),
            _ => throw new ArgumentException("Parameter 'itemKind' has invalid value!", nameof(itemKind)),
        };
        Subject.Add(new Set(new List<ITag>() { seq }));
    }

    public void AddExtensionRequest(ObjectIdentifier reqOid, bool critical, ITag value)
    {
        var ext = new List<ITag>() { reqOid };
        if (critical)
            ext.Add(new BooleanTag(Extensions._booleanTrueSequence));
        ext.Add(value);
        RequestedExtensions.Add(new Sequence(ext));
    }

    public void SetKeyUsage(KeyUsage keyUsage)
    {
        AddExtensionRequest(new ObjectIdentifier(OID.GetOrCreate(OID.KEY_USAGE).ByteValue),
                            keyUsage.Critical,
                            new OctetString(children: new List<ITag>() { new BitString(keyUsage.KeyUsageByteValue) }));
    }

}
