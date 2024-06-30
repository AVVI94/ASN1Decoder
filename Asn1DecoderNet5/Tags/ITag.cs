using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASN1Decoder.NET;
public interface ITag
{
    /// <summary>
    /// The Tag identifier in decimal format
    /// <br/>
    /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
    /// </summary>
    public abstract int TagNumber { get; }
    /// <summary>
    /// The tag identifier in human readable form
    /// <br/>
    /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
    /// </summary>
    public abstract string TagName { get; }
    /// <summary>
    /// Tag class
    /// <br/>
    /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
    /// </summary>
    public abstract int TagClass { get; }
    /// <summary>
    /// Is this tag constructed
    /// </summary>
    public abstract bool IsConstructed { get; }
    /// <summary>
    /// Is this tag universal
    /// </summary>
    public abstract bool IsUniversal { get; }
    /// <summary>
    /// Is this tag EOC (End Of Context)
    /// </summary>
    public abstract bool IsEoc { get; }
    /// <summary>
    /// The content, including the tag byte and the length bytes, of the tag stored in byte array.
    /// </summary>
    public abstract byte[] Content { get; }
    /// <summary>
    /// Collection of <see cref="ITag"/> where every item represents nested tag(s). 
    /// </summary>
    public abstract IList<ITag> Children { get; }
}

public interface IReadOnlyTag : ITag
{
    public new IReadOnlyList<IReadOnlyTag> Children { get; }
    /// <summary>
    /// The content of the tag in human readable form
    /// </summary>
    public string ReadableContent { get; }
    /// <summary>
    /// Converts the <see cref="Content"/> to <see cref="ReadableContent"/>
    /// </summary>
    public void ConvertContentToReadableContent();
}

//public interface IMutableTag : ITag<List<IMutableTag>, IMutableTag>
//{
//    /// <summary>
//    /// The Tag identifier in decimal format
//    /// <br/>
//    /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
//    /// </summary>
//    public abstract int TagNumber { get; set; }
//    /// <summary>
//    /// The tag identifier in human readable form
//    /// <br/>
//    /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
//    /// </summary>
//    public abstract string TagName { get; set; }
//    /// <summary>
//    /// Tag class
//    /// <br/>
//    /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
//    /// </summary>
//    public abstract int TagClass { get; set; }
//    /// <summary>
//    /// Is this tag constructed
//    /// </summary>
//    public abstract bool IsConstructed { get; set; }
//    /// <summary>
//    /// Is this tag universal
//    /// </summary>
//    public abstract bool IsUniversal { get; set; }
//    /// <summary>
//    /// Is this tag EOC (End Of Context)
//    /// </summary>
//    public abstract bool IsEoc { get; set; }
//    /// <summary>
//    /// The content, including the tag byte and the length bytes, of the tag stored in byte array.
//    /// </summary>
//    public abstract byte[] Content { get; set; }
//}

//public interface ITag : ITag<IReadOnlyList<ITag>, ITag>
//{
//    /// <summary>
//    /// The content of the tag in human readable form
//    /// </summary>
//    public string ReadableContent { get; set; }
//    /// <summary>
//    /// Converts the <see cref="Content"/> to <see cref="ReadableContent"/>
//    /// </summary>
//    public void ConvertContentToReadableContent();
//}

public static class Ext
{
    public static IReadOnlyTag ToReadOnlyTag(this ITag tag)
    {
         return Decoder.Decode(Decoder.Encode(tag));
    }
}