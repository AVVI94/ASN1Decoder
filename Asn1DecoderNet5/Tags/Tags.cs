using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using static Asn1DecoderNet5.Tags.ByteArray;

namespace Asn1DecoderNet5.Tags;
#nullable enable
static file class ByteArray
{
#if NET45
    public static readonly byte[] EmptyByteArray = new byte[0];
#else
    public static readonly byte[] EmptyByteArray = Array.Empty<byte>();
#endif
}

public readonly struct Set : IReadOnlyTag
{
    public Set(List<IReadOnlyTag> children)
    {
        Content = EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 49;
    public string TagName { get; } = "SET";
    public int TagClass { get; } = 0;
    public bool IsConstructed => true;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct Eoc : IReadOnlyTag
{
    public Eoc()
    {
        Content = EmptyByteArray;
        Childs = new List<IReadOnlyTag>();
    }
    public int TagNumber { get; } = 0;
    public string TagName { get; } = "EOC";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; } = true;
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct BooleanTag : IReadOnlyTag
{
    public BooleanTag(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = new List<IReadOnlyTag>();
    }
    public int TagNumber { get; } = 1;
    public string TagName { get; } = "BOOLEAN";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct Integer : IReadOnlyTag
{
    public Integer(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = new List<IReadOnlyTag>();
    }
    public int TagNumber { get; } = 2;
    public string TagName { get; } = "INTEGER";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct BitString : IReadOnlyTag
{
    public BitString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 3;
    public string TagName { get; } = "BIT_STRING";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct OctetString : IReadOnlyTag
{
    public OctetString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 4;
    public string TagName { get; } = "OCTET_STRING";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct ObjectIdentifier : IReadOnlyTag
{
    public ObjectIdentifier(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = new List<IReadOnlyTag>();
    }
    public int TagNumber { get; } = 6;
    public string TagName { get; } = "OBJECT_IDENTIFIER";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct Null : IReadOnlyTag
{
    public Null()
    {
        Childs = new List<IReadOnlyTag>();
        Content = EmptyByteArray;
    }
    public int TagNumber { get; } = 5;
    public string TagName { get; } = "NULL";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct ObjectDescriptor : IReadOnlyTag
{
    public ObjectDescriptor(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 7;
    public string TagName { get; } = "ObjectDescriptor";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct External : IReadOnlyTag
{
    public External(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 8;
    public string TagName { get; } = "EXTERNAL";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct Real : IReadOnlyTag
{
    public Real(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = new List<IReadOnlyTag>();
    }
    public int TagNumber { get; } = 9;
    public string TagName { get; } = "REAL";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct Enumerated : IReadOnlyTag
{
    public Enumerated(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = new List<IReadOnlyTag>();
    }
    public int TagNumber { get; } = 10;
    public string TagName { get; } = "ENUMERATED";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct EnumeratedPdv : IReadOnlyTag
{
    public EnumeratedPdv(List<IReadOnlyTag>? children = null)
    {
        Content = EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 10;
    public string TagName { get; } = "ENUMERATED_PDV";
    public int TagClass { get; } = 0;
    public bool IsConstructed => true;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct Utf8String : IReadOnlyTag
{
    public Utf8String(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 12;
    public string TagName { get; } = "UTF8String";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct Sequence : IReadOnlyTag
{
    public Sequence(List<IReadOnlyTag>? children = null)
    {
        Content = EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 48;
    public string TagName { get; } = "SEQUENCE";
    public int TagClass { get; } = 0;
    public bool IsConstructed => true;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct NumericString : IReadOnlyTag
{
    public NumericString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 18;
    public string TagName { get; } = "NumericString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct PritnableString : IReadOnlyTag
{
    public PritnableString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 19;
    public string TagName { get; } = "PrintableString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct TeletextString : IReadOnlyTag
{
    public TeletextString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 20;
    public string TagName { get; } = "TeletextString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct VideotexString : IReadOnlyTag
{
    public VideotexString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 21;
    public string TagName { get; } = "VideotexString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct IA5String : IReadOnlyTag
{
    public IA5String(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 22;
    public string TagName { get; } = "IA5String";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct UTCTime : IReadOnlyTag
{
    public UTCTime(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 23;
    public string TagName { get; } = "UTCTime";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct GeneralizedTime : IReadOnlyTag
{
    public GeneralizedTime(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 24;
    public string TagName { get; } = "GeneralizedTime";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct GraphicString : IReadOnlyTag
{
    public GraphicString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 25;
    public string TagName { get; } = "GraphicString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct VisibleString : IReadOnlyTag
{
    public VisibleString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 26;
    public string TagName { get; } = "VisibleString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct GeneralString : IReadOnlyTag
{
    public GeneralString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 27;
    public string TagName { get; } = "GeneralString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct UniversalString : IReadOnlyTag
{
    public UniversalString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 28;
    public string TagName { get; } = "UniversalString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct BMPString : IReadOnlyTag
{
    public BMPString(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; } = 30;
    public string TagName { get; } = "BMPString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct UniversalTag : IReadOnlyTag
{
    public UniversalTag(int tagNumber, byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        TagNumber = tagNumber;
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; }
    public string TagName => "Universal_" + TagNumber;
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct ContextSpecificTag : IReadOnlyTag
{
    public ContextSpecificTag(int tagNumber, byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        TagNumber = tagNumber;
        TagName = $"[{tagNumber & 0x1F}]";
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct ContextSpecific_0 : IReadOnlyTag
{
    public ContextSpecific_0(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        TagNumber = 160;
        TagName = $"[0]";
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct ContextSpecific_1 : IReadOnlyTag
{
    public ContextSpecific_1(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        TagNumber = 161;
        TagName = $"[0]";
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct ContextSpecific_2 : IReadOnlyTag
{
    public ContextSpecific_2(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        TagNumber = 162;
        TagName = $"[2]";
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
public readonly struct ContextSpecific_3 : IReadOnlyTag
{
    public ContextSpecific_3(byte[]? content = null, List<IReadOnlyTag>? children = null)
    {
        TagNumber = 163;
        TagName = $"[3]";
        Content = content ?? EmptyByteArray;
        Childs = children ?? new();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Childs.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IReadOnlyList<IReadOnlyTag> Childs { get; }
    public byte[] Content { get; }
}
