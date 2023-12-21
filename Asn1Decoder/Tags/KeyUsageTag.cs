using System;
using System.Collections.Generic;
using System.Linq;
using Asn1DecoderNet5.Interfaces;
namespace Asn1DecoderNet5.Tags;
/// <summary>
/// Tag representing KeyUsage BIT_STRING tag
/// </summary>
public struct KeyUsageTag : ITag
{
    /// <summary>
    /// Tag representing KeyUsage BIT_STRING tag
    /// </summary>
    /// <param name="genericTag">BIT_STRING tag base</param>
    /// <exception cref="ArgumentException">Thrown if the genericTag is not BIT_STRING</exception>
    public KeyUsageTag(in ITag genericTag, bool critical)
    {
        if (genericTag.TagNumber != (int)Tags.BIT_STRING)
            throw new ArgumentException("KeyUsage tag must be based on BitString tag!");

        TagNumber = genericTag.TagNumber;
        TagName = genericTag.TagName;
        TagClass = genericTag.TagClass;
        IsConstructed = genericTag.IsConstructed;
        IsUniversal = genericTag.IsUniversal;
        IsEoc = genericTag.IsEoc;
        Childs = genericTag.Childs;
        Content = genericTag.Content;
        ReadableContent = genericTag.ReadableContent;

        var bits = Tag.ParseBitString(Content).PadRight(8, '0');
        var decipherOnly = bits == "00000000";
        var ks = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            ks[i] = bits[i] == '1';
        }
        KeyUsage = new KeyUsage(ks, critical, decipherOnly);
    }
    public int TagNumber { get; set; }
    public string TagName { get; set; }
    public int TagClass { get; set; }
    public readonly bool IsConstructed { get; }
    public readonly bool IsUniversal { get; }
    public readonly bool IsEoc { get; }
    public List<ITag> Childs { get; set; }
    public byte[] Content { get; set; }
    public string ReadableContent { get; set; }

    public readonly KeyUsage KeyUsage { get; }

    public void ConvertContentToReadableContent()
    {
        ReadableContent = Tag.ParseBitString(Content);
    }
}
/// <summary>
/// Struct representing key usage values
/// </summary>
public readonly struct KeyUsage
{
    public KeyUsage(bool[] keyUsages, bool critical, bool decipherOnly) : this()
    {
        KeyUsages = keyUsages;
        Critical = critical;
        DecipherOnly = decipherOnly;
    }
#nullable enable
    public readonly bool[]? KeyUsages { get; }
#nullable restore

    public readonly bool DigitalSignature => KeyUsages[0];
    public readonly bool NonRepudiation => KeyUsages[1];
    public readonly bool KeyEncipherment => KeyUsages[2];
    public readonly bool DataEncipherment => KeyUsages[3];
    public readonly bool KeyAgreement => KeyUsages[4];
    /// <summary>
    /// CodeSign
    /// </summary>
    public readonly bool KeyCertSign => KeyUsages[5];
    public readonly bool CRLSign => KeyUsages[6];
    public readonly bool EncipherOnly => KeyUsages[7];
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
