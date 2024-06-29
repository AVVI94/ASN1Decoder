using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;

namespace Asn1DecoderNet5.Tags.SAN;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
public enum SanItemKind
{
    OtherName,
    Rfc822Name,
    DNSName,
    X400Address,
    DirectoryName,
    EdiPartyName,
    UniformResourceIdentifier,
    IPAddress,
    RegisteredID,
}

/// <summary>
/// SubjectAlternativeName item
/// </summary>
/// <typeparam name="T">Item's content type</typeparam>
public interface ISanItem
{
    /// <summary>
    /// Item's kind
    /// </summary>
    SanItemKind ItemKind { get; }
}

public abstract class OtherNameBase : ISanItem
{
    /// <summary>
    /// Item's object identifier
    /// </summary>
    public OID Oid { get; protected set; }
    public SanItemKind ItemKind => SanItemKind.OtherName;
}

/// <summary>
/// Abstract base for OtherNames
/// </summary>
/// <typeparam name="T">OtherName content type</typeparam>
public abstract class OtherNameBase<T> : OtherNameBase
{
    public T Content { get; protected set; }
}

/// <summary>
/// OtherName SAN Item
/// </summary>
public class OtherName : OtherNameBase<ITag>
{
    public OtherName(OID oid, ITag content)
    {
        Oid = oid;
        Content = content;
    }
}

public class IcaUserId : OtherNameBase<string>
{
    public IcaUserId(ITag content)
    {
        Oid = OID.GetOrCreate(OID.ICA_USER_ID);
        content.Childs[0].ConvertContentToReadableContent();
        Content = content.Childs[0].ReadableContent;
    }
}

public class IcaIkMpsv : OtherNameBase<string>
{
    public IcaIkMpsv(ITag content)
    {
        Oid = OID.GetOrCreate(OID.ICA_IK_MPSV);
        content.Childs[0].ConvertContentToReadableContent();
        Content = content.Childs[0].ReadableContent;
    }
}

/// <summary>
/// Rfc822Name SAN Item
/// </summary>
public class Rfc822Name : ISanItem
{
    public Rfc822Name(string content)
    {
        Content = content;
    }
    public SanItemKind ItemKind => SanItemKind.Rfc822Name;
    public string Content { get; }
}
/// <summary>
/// DnsName SAN Item
/// </summary>
public class DnsName : ISanItem
{
    public DnsName(string content)
    {
        Content = content;
    }
    public SanItemKind ItemKind => SanItemKind.DNSName;
    public string Content { get; }
}

/// <summary>
/// X400Address SAN Item, unsupported, Content is not parsed
/// </summary>
public class X400Address : ISanItem
{
    public X400Address(ITag content)
    {
        Content = content;
    }
    public SanItemKind ItemKind => SanItemKind.X400Address;
    /// <summary>
    /// X400Address is unsupported, the content is not parsed and is provided as ITag so you can parse it yourself
    /// </summary>
    public ITag Content { get; }
}

/// <summary>
/// DirectoryName SAN Item
/// </summary>
public class DirectoryName : ISanItem
{
    public DirectoryName(ITag content)
    {
        Content = new List<RelativeDistinguishedName>();
        foreach (var set in content.Childs)
        {
            if (string.IsNullOrEmpty(set.Childs[0].Childs[1].ReadableContent))
                set.Childs[0].Childs[1].ConvertContentToReadableContent();
            Content.Add(new RelativeDistinguishedName(
             OID.GetOrCreate(set.Childs[0].Childs[0].Content),
             set.Childs[0].Childs[1].ReadableContent
             ));
        }
    }
    public SanItemKind ItemKind => SanItemKind.DirectoryName;
    public List<RelativeDistinguishedName> Content { get; }
}

/// <summary>
/// EidPartyName SAN Item
/// </summary>
public class EdiPartyName : ISanItem
{
    // EDIPartyName ::= SEQUENCE {
    //        nameAssigner            [0]     DirectoryString OPTIONAL,
    //        partyName               [1]     DirectoryString }
    public EdiPartyName(ITag content)
    {
        if (content.Childs.Count == 2)
        {
            content.Childs[0].ConvertContentToReadableContent();
            NameAssigner = content.Childs[0].ReadableContent;

            content.Childs[1].ConvertContentToReadableContent();
            PartyName = content.Childs[1].ReadableContent;
        }
        else
        {
            content.Childs[0].ConvertContentToReadableContent();
            PartyName = content.Childs[0].ReadableContent;
        }
    }
    public SanItemKind ItemKind => SanItemKind.EdiPartyName;
    public string? NameAssigner { get; }
    public string PartyName { get; }
}

/// <summary>
/// URI SAN Item
/// </summary>
public class UniformResourceIdentifier : ISanItem
{
    public UniformResourceIdentifier(string content)
    {
        Content = content;
    }
    public SanItemKind ItemKind => SanItemKind.UniformResourceIdentifier;
    public string Content { get; }
}
/// <summary>
/// IPAddress SAN Item
/// </summary>
public class IPAddress : ISanItem
{
    public IPAddress(string content)
    {
        Content = content;
    }
    public SanItemKind ItemKind => SanItemKind.IPAddress;
    public string Content { get; }
}
/// <summary>
/// RegisteredID SAN Item
/// </summary>
public class RegisteredID : ISanItem
{
    public RegisteredID(OID content)
    {
        Content = content;
    }
    public SanItemKind ItemKind => SanItemKind.RegisteredID;
    public OID Content { get; }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
