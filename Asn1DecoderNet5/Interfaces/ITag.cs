using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Tags;

namespace Asn1DecoderNet5.Interfaces
{
    public interface ITag
    {
        int TagNumber { get; set; }
        string TagName { get; set; }
        int TagClass { get; set; }
        bool IsConstructed { get; }
        List<ITag> Childs { get; set; }
        byte[] Content { get; set; }
        public string ReadableContent { get; set; }
        bool IsUniversal();
        bool IsEoc();
        void ConvertContentToReadableContent();
    }
}
