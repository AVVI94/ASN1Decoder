using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;
namespace Asn1DecoderNet5.CSR;

#nullable enable
public class CertificationRequest
{

    public CertificationRequest(OID signatureAlgorithmOid, HashAlgorithm signatureHashAlgorithm)
    {
        PublicKeyInfo = new Sequence(new List<IReadOnlyTag>()
        {
            new Sequence(new List<IReadOnlyTag>()
            {
                new ObjectIdentifier(OID.GetOrCreate(OID.RSA_ENCRYPTION).ByteValue),
                new Null()
            })
        });
        SignatureAlgorithmOid = signatureAlgorithmOid;
        SignatureHashAlgorithm = signatureHashAlgorithm;
    }

    public List<Set> Subject { get; } = new();
    public List<IReadOnlyTag> RequestedExtensions { get; } = new();
    public List<IReadOnlyTag> OtherElementsAfterExtensionRequestSequence { get; } = new();
    public OID SignatureAlgorithmOid { get; private set; }
    public HashAlgorithm SignatureHashAlgorithm { get; private set; }
    public Sequence PublicKeyInfo { get; private set; }
    //public AsymmetricAlgorithm? KeyPair { get; set; }

    public byte[] BuildRequest(RSACryptoServiceProvider keyPair)
    {
        var version = new Integer(new byte[1] { 0x0 });
        var subject = new Sequence(Subject.Cast<IReadOnlyTag>().ToList());
        var keyAlg = new Sequence(new List<IReadOnlyTag>()
        {
            new ObjectIdentifier(SignatureAlgorithmOid.ByteValue),
            new Null()
        });

        var keyParams = keyPair.ExportParameters(false);

        var key = new Sequence(new List<IReadOnlyTag>()
        {
            new Integer(keyParams.Modulus),
            new Integer(keyParams.Exponent),
        });
        var bkey = new List<byte>() { 0x0, 0x0 };
        bkey.AddRange(Decoder.Encode(key));

        ((List<IReadOnlyTag>)PublicKeyInfo.Childs).Add(new BitString(bkey.ToArray()));
        
        var ext = new ContextSpecific_0(null, new List<IReadOnlyTag>()
        {
            new ObjectIdentifier(OID.OidDictionary[OID.EXTENSION_REQUEST].ByteValue),
            new Set(new List<IReadOnlyTag>()
            {
                new Sequence(RequestedExtensions)
            })
        });
        var certRequestInfo = new Sequence(new List<IReadOnlyTag>()
            {
                version,
                subject,
                PublicKeyInfo,
                ext
            });
        var toBeSigned = Decoder.Encode(certRequestInfo);
        var signature = keyPair.SignData(toBeSigned, SignatureHashAlgorithm);
        var request = new Sequence(new List<IReadOnlyTag>()
        {
            certRequestInfo,
            keyAlg,
            new BitString(signature)
        });
        return Decoder.Encode(request);
    }

    public void AddSubjectItem(SubjectItemKind itemKind, byte[] value, bool useUtf8StringValueTag)
    {
        static Sequence CreateSeq(IReadOnlyTag oid, IReadOnlyTag value) => new(new List<IReadOnlyTag>() { oid, value });
        static ObjectIdentifier CreateOid(SubjectItemKind itemKind) => new(OID.GetOrCreate(itemKind.ToOidString()).ByteValue);
        static IReadOnlyTag CreateVal(byte[] val, bool utf8) => utf8 ? new Utf8String(val) : new PritnableString(val);

        var seq = itemKind switch
        {
            SubjectItemKind.CommonName => CreateSeq(CreateOid(SubjectItemKind.CommonName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.GivenName => CreateSeq(CreateOid(SubjectItemKind.GivenName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.Surname => CreateSeq(CreateOid(SubjectItemKind.Surname), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.CountryName => CreateSeq(CreateOid(SubjectItemKind.CountryName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.OrganizationName => CreateSeq(CreateOid(SubjectItemKind.OrganizationName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.OrganizationUnit => CreateSeq(CreateOid(SubjectItemKind.OrganizationUnit), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.StateOrProvinceName => CreateSeq(CreateOid(SubjectItemKind.StateOrProvinceName), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.Locality => CreateSeq(CreateOid(SubjectItemKind.Locality), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.SerialNumber => CreateSeq(CreateOid(SubjectItemKind.SerialNumber), CreateVal(value, useUtf8StringValueTag)),
            SubjectItemKind.Title => CreateSeq(CreateOid(SubjectItemKind.Title), CreateVal(value, useUtf8StringValueTag)),
            _ => throw new ArgumentException("Parameter 'itemKind' has invalid value!", nameof(itemKind)),
        };
        Subject.Add(new Set(new List<IReadOnlyTag>() { seq }));
    }

    public void AddExtensionRequest(ObjectIdentifier reqOid, bool critical, IReadOnlyTag value)
    {
        var ext = new List<IReadOnlyTag>() { reqOid };
        if (critical)
            ext.Add(new BooleanTag(Extensions._booleanTrueSequence));
        ext.Add(value);
        RequestedExtensions.Add(new Sequence(ext));
    }

    public void SetKeyUsage(KeyUsage keyUsage)
    {
        var ba = new BitArray(keyUsage.KeyUsages);
        var bytes = new byte[1];
        ba.CopyTo(bytes, 0);
        AddExtensionRequest(new ObjectIdentifier(OID.GetOrCreate(OID.KEY_USAGE).ByteValue),
                            keyUsage.Critical,
                            new OctetString(children: new List<IReadOnlyTag>() { new BitString(bytes) }));
    }
}
