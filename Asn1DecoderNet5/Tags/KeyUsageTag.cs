using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace ASN1Decoder.NET.Tags;
/// <summary>
/// Tag representing KeyUsage BIT_STRING tag
/// </summary>
public struct KeyUsageTag : IReadOnlyTag
{
    /// <summary>
    /// Tag representing KeyUsage BIT_STRING tag
    /// </summary>
    /// <param name="genericTag">BIT_STRING tag base</param>
    /// <exception cref="ArgumentException">Thrown if the genericTag is not BIT_STRING</exception>
    public KeyUsageTag(in IReadOnlyTag genericTag, bool critical)
    {
        if (genericTag.TagNumber != (int)TagNames.BIT_STRING)
            throw new ArgumentException("KeyUsage tag must be based on BitString tag!");

        TagNumber = genericTag.TagNumber;
        TagName = genericTag.TagName;
        TagClass = genericTag.TagClass;
        IsConstructed = genericTag.IsConstructed;
        IsUniversal = genericTag.IsUniversal;
        IsEoc = genericTag.IsEoc;
        Children = genericTag.Children;
        Content = genericTag.Content;
        ReadableContent = genericTag.ReadableContent;

        var bits = SmartTag.ParseBitString(Content).PadRight(8, '0');
        var decipherOnly = bits == "00000000";
        var ks = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            ks[i] = bits[i] == '1';
        }
        KeyUsage = new KeyUsage(ks, critical, decipherOnly);
    }
    public readonly int TagNumber { get; }
    public readonly string TagName { get; }
    public readonly int TagClass { get; }
    public readonly bool IsConstructed { get; }
    public readonly bool IsUniversal { get; }
    public readonly bool IsEoc { get; }
    public readonly IReadOnlyList<IReadOnlyTag> Children { get; }
    public readonly byte[] Content { get; }
    public string ReadableContent { get; private set; }

    public readonly KeyUsage KeyUsage { get; }
    readonly IList<ITag> ITag.Children => (IList<ITag>)Children;

    public void ConvertContentToReadableContent()
    {
        ReadableContent = SmartTag.ParseBitString(Content);
    }
}
/// <summary>
/// Struct representing key usage values
/// </summary>
public readonly struct KeyUsage
{
    internal KeyUsage(bool[] keyUsages, bool critical, bool decipherOnly) : this()
    {
        KeyUsages = keyUsages;
        Critical = critical;
        DecipherOnly = decipherOnly;
    }
    /// <summary>
    /// Creates new KeyUsage representation, using this variant the KeyUsage will be set to DecipherOnly!
    /// </summary>
    /// <param name="critical"></param>
    public KeyUsage(bool critical)
    {
        DecipherOnly = true;
        KeyUsages = new bool[8];
        Critical = critical;
    }
    public KeyUsage(bool critical,
                    bool digitalSignature = false,
                    bool nonRepudiation = false,
                    bool keyEncipherment = false,
                    bool dataEncipherment = false,
                    bool keyAgreement = false,
                    bool keyCertSign = false,
                    bool cRLSign = false,
                    bool encipherOnly = false)
    {
        Critical = critical;
        KeyUsages =
        [
            digitalSignature,
            nonRepudiation,
            keyEncipherment,
            dataEncipherment,
            keyAgreement,
            keyCertSign,
            cRLSign,
            encipherOnly,
        ];
    }
#nullable enable
    public readonly bool[]? KeyUsages { get; }
    public byte KeyUsageByteValue
    {
        get
        {
            if (KeyUsages is null)
                return 0;
            byte result = 0;

            for (int i = 0; i < 8; i++)
            {
                if (KeyUsages[i])
                {
                    result |= (byte)(1 << (7 - i));
                }
            }

            return result;
        }
    }
#nullable restore

    public readonly bool DigitalSignature => KeyUsages?[0] ?? false;
    public readonly bool NonRepudiation => KeyUsages?[1] ?? false;
    public readonly bool KeyEncipherment => KeyUsages?[2] ?? false;
    public readonly bool DataEncipherment => KeyUsages?[3] ?? false;
    public readonly bool KeyAgreement => KeyUsages?[4] ?? false;
    /// <summary>
    /// CodeSign
    /// </summary>
    public readonly bool KeyCertSign => KeyUsages?[5] ?? false;
    public readonly bool CRLSign => KeyUsages?[6] ?? false;
    public readonly bool EncipherOnly => KeyUsages?[7] ?? false;
    public readonly bool DecipherOnly { get; }
    public readonly bool Critical { get; }

    public override bool Equals(object obj)
    {
        if (obj is not KeyUsage k)
            return false;

        return KeyUsages.SequenceEqual(k.KeyUsages);
    }

    public static bool operator ==(KeyUsage left, KeyUsage right) => left.Equals(right);
    public static bool operator !=(KeyUsage left, KeyUsage right) => !left.Equals(right);
    public override int GetHashCode()
    {
        return KeyUsages.GetHashCode() + Critical.GetHashCode() + DecipherOnly.GetHashCode();
    }
}
