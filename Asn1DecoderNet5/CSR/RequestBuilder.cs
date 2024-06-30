using ASN1Decoder.NET.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASN1Decoder.NET.CSR;
#nullable enable
public class RequestBuilder
{
    List<ITag> _subject = [];
    List<ITag> _requestedExtensions = [];
    List<ITag> _eku = [];

    public RequestBuilder WithSubject(Action<SubjectBuilder> subjectBuilder)
    {
        var builder = new SubjectBuilder();
        subjectBuilder(builder);
        _subject = builder._subject;
        return this;
    }

    public RequestBuilder WithSubjectItem(SubjectItemKind kind, string value, bool printableString = false)
    {
        if (printableString)
        {
            PrintableString.EnsureValidValue(value);
        }
        var val = printableString == false ? Encoding.UTF8.GetBytes(value) : Encoding.ASCII.GetBytes(value);
        _subject.Add(SubjectBuilder.CreateSubjectSequence(kind, val, printableString));
        return this;
    }

    public RequestBuilder WithSubjectItem(ITag item)
    {
        _subject.Add(item);
        return this;
    }

    public RequestBuilder SetKeyUsage(KeyUsage keyUsage)
    {
        WithExtensionRequest(new ObjectIdentifier(OID.GetOrCreate(OID.KEY_USAGE).ByteValue),
                            keyUsage.Critical,
                            new OctetString(children: new List<ITag>() { new BitString(keyUsage.KeyUsageByteValue) }));
        return this;
    }

    public RequestBuilder WithExtendedKeyUsage(string oid)
    {
        _eku.Add(new ObjectIdentifier(OID.GetOrCreate(oid).ByteValue));
        return this;
    }

    public RequestBuilder WithExtendedKeyUsage(ObjectIdentifier oid)
    {
        _eku.Add(oid);
        return this;
    }

    public RequestBuilder WithExtendedKeyUsage(byte[] oid)
    {
        _eku.Add(new ObjectIdentifier(oid));
        return this;
    }

    public void WithExtensionRequest(ObjectIdentifier reqOid, bool critical, ITag value)
    {
        var ext = new List<ITag>() { reqOid };
        if (critical)
            ext.Add(new BooleanTag(Extensions._booleanTrueSequence));
        ext.Add(value);
        _requestedExtensions.Add(new Sequence(ext));
    }

    public RequestBuilder WithExtensionRequest(Sequence extension)
    {
        _requestedExtensions.Add(extension);
        return this;
    }

    Sequence BuildPublicKeyInfo(byte[] encryptionOid, byte[]? curveOid, byte[] mod_x, byte[] exp_y)
    {
        var sq = new Sequence([
            new Integer(mod_x),
            new Integer(exp_y)
            ]);
        var bytes = Decoder.Encode(sq);
        return new Sequence(
        [
            new Sequence(
            [
                new ObjectIdentifier(encryptionOid),
                curveOid is null ? new Null() : new ObjectIdentifier(curveOid)
            ]),
            new BitString(bytes)
        ]);
    }

    ContextSpecific_0? BuildExtensionRequest()
    {
        if (_requestedExtensions.Count == 0)
            return null;
        _ = WithExtensionRequest(new Sequence([
                new ObjectIdentifier(OID.GetOrCreate(OID.EXT_KEY_USAGE).ByteValue),
                new OctetString(null, [new Sequence(_eku)])
            ]));
        return new ContextSpecific_0(null,
        [
            new ObjectIdentifier(OID.GetOrCreate(OID.EXTENSION_REQUEST).ByteValue),
            new Set(
                [
                new Sequence(_requestedExtensions)
            ])
        ]);
    }

    public byte[] BuildCSR(byte[] modulus,
                           byte[] exponent,
                           OID signatureAlgorithm,
                           OID keyAlgorithm,
                           HashAlgorithmName signatureHashAlgorithm,
                           Func<HashAlgorithmName, byte[], byte[]> signDataCallback)
    {
        var version = new Integer([0]);
        var subject = new Sequence(_subject);
        var publicKeyInfo = BuildPublicKeyInfo(keyAlgorithm.ByteValue,
                                               signatureAlgorithm.ByteValue,
                                               modulus,
                                               exponent);
        var ext = BuildExtensionRequest();
        var certRequestInfo = new Sequence([
            version,
            subject,
            publicKeyInfo,
            ext
            ]);
        var toBeSigned = Decoder.Encode(certRequestInfo);
        var signature = signDataCallback(signatureHashAlgorithm, toBeSigned);
        var request = new Sequence([certRequestInfo, new ObjectIdentifier(signatureAlgorithm.ByteValue), new BitString(signature)]);
        return Decoder.Encode(request);
    }

    public byte[] BuildCSR(ECDsa cng, OID signatureAlgorithm, HashAlgorithmName signatureHashAlgorithm)
    {
        var pr = cng.ExportParameters(false);
        var curve = OID.GetOrCreate(pr.Curve.Oid.Value);        
        var mod = pr.Q.X;
        var exp = pr.Q.Y;
        return BuildCSR(mod,
                        exp,
                        signatureAlgorithm,
                        OID.GetOrCreate(OID.EC_PUBLIC_KEY),
                         signatureHashAlgorithm,
                        (hash, data) => cng.SignData(data, hash));
    }

    public byte[] BuildCSR(RSA rsa, OID signatureAlgorithm, HashAlgorithmName signatureHashAlgorithm)
    {
        var pr = rsa.ExportParameters(false);
        var mod = pr.Modulus;
        var exp = pr.Exponent;
        return BuildCSR(mod,
                        exp,
                        signatureAlgorithm,
                        OID.GetOrCreate(OID.RSA_ENCRYPTION),
                        signatureHashAlgorithm,
                        (hash, data) => rsa.SignData(data, hash, RSASignaturePadding.Pkcs1));
    }
}

public class SubjectBuilder
{
    internal List<ITag> _subject = [];

    public SubjectBuilder CommonName(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.CommonName, value, printableString));
        return this;
    }

    public SubjectBuilder Surname(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.Surname, value, printableString));
        return this;
    }

    public SubjectBuilder GivenName(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.GivenName, value, printableString));
        return this;
    }

    public SubjectBuilder CountryName(string value)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.CountryName, value, true));
        return this;
    }

    public SubjectBuilder OrganizationName(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.OrganizationName, value, printableString));
        return this;
    }

    public SubjectBuilder OrganizationalUnitName(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.OrganizationUnitName, value, printableString));
        return this;
    }

    public SubjectBuilder LocalityName(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.LocalityName, value, printableString));
        return this;
    }

    public SubjectBuilder StateOrProvinceName(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.StateOrProvinceName, value, printableString));
        return this;
    }

    public SubjectBuilder StreetAddress(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.StreetAddress, value, printableString));
        return this;
    }

    public SubjectBuilder PostalCode(string value, bool printableString = false)
    {
        _subject.Add(CreateSubjectSequence(SubjectItemKind.PostalCode, value, printableString));
        return this;
    }

    internal static Sequence CreateSubjectSequence(SubjectItemKind itemKind, string value, bool printableString = false)
        => CreateSubjectSequence(itemKind, printableString == false ? Encoding.UTF8.GetBytes(value) : Encoding.ASCII.GetBytes(value), printableString);

    internal static Sequence CreateSubjectSequence(SubjectItemKind itemKind, byte[] value, bool printableString)
    {
        var oid = new ObjectIdentifier(OID.GetOrCreate(itemKind.ToOidString()).ByteValue);
        ITag val = printableString == false ? new Utf8String(value) : new PrintableString(value);
        return new Sequence([oid, val]);
    }
}