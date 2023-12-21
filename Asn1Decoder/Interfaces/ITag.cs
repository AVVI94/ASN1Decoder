using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1Decoder.Tags;

namespace Asn1Decoder.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// The Tag identifier in decimal format
        /// <br/>
        /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
        /// </summary>
        int TagNumber { get; set; }

        /// <summary>
        /// The tag identifier in human readable form
        /// <br/>
        /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
        /// </summary>
        string TagName { get; set; }

        /// <summary>
        /// Tag class
        /// <br/>
        /// For more information visit: <see href="https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-encoded-tag-bytes"/>
        /// </summary>
        int TagClass { get; set; }

        /// <summary>
        /// Is this tag constructed
        /// </summary>
        bool IsConstructed { get; }
        /// <summary>
        /// Is this tag universal
        /// </summary>
        bool IsUniversal { get; }
        /// <summary>
        /// Is this tag EOC (End Of Context)
        /// </summary>
        bool IsEoc { get; }

        /// <summary>
        /// List of <see cref="ITag"/> where every item represents nested tag(s). When this list is empty, then this tag SHOULD have some kind of <see cref="ReadableContent"/>.
        /// </summary>
        List<ITag> Childs { get; set; }

        /// <summary>
        /// The content, including the tag byte and the length bytes, of the tag stored in byte array.
        /// </summary>
        byte[] Content { get; set; }

        /// <summary>
        /// Human readable form of actual content, this should only be set when the <see cref="Childs"/> list is empty.
        /// </summary>
        string ReadableContent { get; set; }

        /// <summary>
        /// Converts the content from bytes to readable string
        /// </summary>
        void ConvertContentToReadableContent();
    }
}
