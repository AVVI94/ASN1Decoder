using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asn1DecoderNet5.Interfaces
{
    public interface IASN1
    {
        byte[] Data { get; set; }
        int HeaderBytesCount { get; set; }
        int Length { get; set; }
        ITag Tag { get; set; }

        string TypeName();
        string Content();
        int StartPosition();
        int ContentPosition();
        int EndPosition();
        int LengthPosition();
        int DecodeLength();
    }
}
