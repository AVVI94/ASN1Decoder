using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ASN1Decoder.NET.Tags.ByteArray;

namespace ASN1Decoder.NET.Tags;
#nullable enable
static file class ByteArray
{
#if NET45
    public static readonly byte[] EmptyByteArray = new byte[0];
#else
    public static readonly byte[] EmptyByteArray = Array.Empty<byte>();
#endif
}

public class Set : ITag
{
    public Set(List<ITag> children)
    {
        Content = EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 49;
    public string TagName { get; } = "SET";
    public int TagClass { get; } = 0;
    public bool IsConstructed => true;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class Eoc : ITag
{
    public Eoc()
    {
        Content = EmptyByteArray;
        Children = new List<ITag>();
    }
    public int TagNumber { get; } = 0;
    public string TagName { get; } = "EOC";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; } = true;
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class BooleanTag : ITag
{
    public BooleanTag(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Children = new List<ITag>();
    }
    public int TagNumber { get; } = 1;
    public string TagName { get; } = "BOOLEAN";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class Integer : ITag
{
    public Integer(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Children = new List<ITag>();
    }
    public int TagNumber { get; } = 2;
    public string TagName { get; } = "INTEGER";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class BitString : ITag
{
    public BitString(byte[]? content = null, IList<ITag>? children = null)
    {
        if (content is not null)
        {
            var _ = CalculateUnusedBits(content);
            Content = [(byte)_, .. content];
        }
        else
            Content = EmptyByteArray;
        //Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public BitString(byte content) : this([content], null) { }

    public int TagNumber { get; } = 3;
    public string TagName { get; } = "BIT_STRING";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }

    public static int CalculateUnusedBits(byte[] bitStringBytes)
    {
        byte lastByte = bitStringBytes[bitStringBytes.Length - 1];
        int unusedBits = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((lastByte & (1 << i)) != 0)
            {
                unusedBits = 7 - i;
                break;
            }
        }
        return unusedBits;
    }
}
public class OctetString : ITag
{
    public OctetString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 4;
    public string TagName { get; } = "OCTET_STRING";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class ObjectIdentifier : ITag
{
    public ObjectIdentifier(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Children = new List<ITag>();
    }
    public int TagNumber { get; } = 6;
    public string TagName { get; } = "OBJECT_IDENTIFIER";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class Null : ITag
{
    public Null()
    {
        Children = new List<ITag>();
        Content = EmptyByteArray;
    }
    public int TagNumber { get; } = 5;
    public string TagName { get; } = "NULL";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class ObjectDescriptor : ITag
{
    public ObjectDescriptor(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 7;
    public string TagName { get; } = "ObjectDescriptor";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class External : ITag
{
    public External(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 8;
    public string TagName { get; } = "EXTERNAL";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class Real : ITag
{
    public Real(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Children = new List<ITag>();
    }
    public int TagNumber { get; } = 9;
    public string TagName { get; } = "REAL";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class Enumerated : ITag
{
    public Enumerated(byte[]? content = null)
    {
        Content = content ?? EmptyByteArray;
        Children = new List<ITag>();
    }
    public int TagNumber { get; } = 10;
    public string TagName { get; } = "ENUMERATED";
    public int TagClass { get; } = 0;
    public bool IsConstructed { get; } = false;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class EnumeratedPdv : ITag
{
    public EnumeratedPdv(List<ITag>? children = null)
    {
        Content = EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 10;
    public string TagName { get; } = "ENUMERATED_PDV";
    public int TagClass { get; } = 0;
    public bool IsConstructed => true;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class Utf8String : ITag
{
    public Utf8String(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 12;
    public string TagName { get; } = "UTF8String";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class Sequence : ITag
{
    public Sequence(List<ITag>? children = null)
    {
        Content = EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 48;
    public string TagName { get; } = "SEQUENCE";
    public int TagClass { get; } = 0;
    public bool IsConstructed => true;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class NumericString : ITag
{
    public NumericString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 18;
    public string TagName { get; } = "NumericString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class PrintableString : ITag
{
    public PrintableString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 19;
    public string TagName { get; } = "PrintableString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }

    public static void EnsureValidValue(string input)
    {
        // Check each character in the input string
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (!IsAllowedCharacter(c))
            {
                throw new ArgumentException($"Invalid character '{c}' at index {i}. See ASN.1 PrintableString definition!", "value");
            }
        }
    }

    private static bool IsAllowedCharacter(char c)
    {
        // Check if the character is within the allowed ASCII ranges for PrintableString
        return (c >= 'A' && c <= 'Z') || // Latin capital letters
               (c >= 'a' && c <= 'z') || // Latin small letters
               (c >= '0' && c <= '9') || // Numbers
               c == ' ' || // SPACE
               c == '\'' || // APOSTROPHE
               c == '(' || // LEFT PARENTHESIS
               c == ')' || // RIGHT PARENTHESIS
               c == '+' || // PLUS SIGN
               c == ',' || // COMMA
               c == '-' || // HYPHEN-MINUS
               c == '.' || // FULL STOP
               c == '/' || // SOLIDUS
               c == ':' || // COLON
               c == '=' || // EQUALS SIGN
               c == '?';   // QUESTION MARK
    }
}
public class TeletextString : ITag
{
    public TeletextString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 20;
    public string TagName { get; } = "TeletextString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class VideotexString : ITag
{
    public VideotexString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 21;
    public string TagName { get; } = "VideotexString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class IA5String : ITag
{
    public IA5String(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 22;
    public string TagName { get; } = "IA5String";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class UTCTime : ITag
{
    public UTCTime(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 23;
    public string TagName { get; } = "UTCTime";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class GeneralizedTime : ITag
{
    public GeneralizedTime(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 24;
    public string TagName { get; } = "GeneralizedTime";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class GraphicString : ITag
{
    public GraphicString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 25;
    public string TagName { get; } = "GraphicString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class VisibleString : ITag
{
    public VisibleString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 26;
    public string TagName { get; } = "VisibleString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class GeneralString : ITag
{
    public GeneralString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 27;
    public string TagName { get; } = "GeneralString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class UniversalString : ITag
{
    public UniversalString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 28;
    public string TagName { get; } = "UniversalString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class BMPString : ITag
{
    public BMPString(byte[]? content = null, IList<ITag>? children = null)
    {
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; } = 30;
    public string TagName { get; } = "BMPString";
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class UniversalTag : ITag
{
    public UniversalTag(int tagNumber, byte[]? content = null, IList<ITag>? children = null)
    {
        TagNumber = tagNumber;
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; }
    public string TagName => "Universal_" + TagNumber;
    public int TagClass { get; } = 0;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; } = true;
    public bool IsEoc { get; }
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class ContextSpecificTag : ITag
{
    public ContextSpecificTag(int tagNumber, byte[]? content = null, IList<ITag>? children = null)
    {
        TagNumber = tagNumber;
        TagName = $"[{tagNumber & 0x1F}]";
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class ContextSpecific_0 : ITag
{
    public ContextSpecific_0(byte[]? content = null, IList<ITag>? children = null)
    {
        TagNumber = 160;
        TagName = $"[0]";
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class ContextSpecific_1 : ITag
{
    public ContextSpecific_1(byte[]? content = null, IList<ITag>? children = null)
    {
        TagNumber = 161;
        TagName = $"[0]";
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class ContextSpecific_2 : ITag
{
    public ContextSpecific_2(byte[]? content = null, IList<ITag>? children = null)
    {
        TagNumber = 162;
        TagName = $"[2]";
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
public class ContextSpecific_3 : ITag
{
    public ContextSpecific_3(byte[]? content = null, IList<ITag>? children = null)
    {
        TagNumber = 163;
        TagName = $"[3]";
        Content = content ?? EmptyByteArray;
        Children = children ?? new List<ITag>();
    }
    public int TagNumber { get; }
    public string TagName { get; }
    public int TagClass { get; } = 2;
    public bool IsConstructed => Content.Length != 0 && Children.Count == 0;
    public bool IsUniversal { get; }
    public bool IsEoc { get; } = false;
    public IList<ITag> Children { get; }
    public byte[] Content { get; }
}
