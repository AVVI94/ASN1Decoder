using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;

namespace Asn1DecoderNet5
{
    public class ASN1 : IASN1
    {
        public ASN1(byte[] stream, int headerBytesCount, ITag tag = null)
        {
            Data = stream;
            HeaderBytesCount = headerBytesCount;
            Tag = tag;
            Length = this.DecodeLength();
        }
        public byte[] Data { get; set; }
        public int HeaderBytesCount { get; set; }
        public int Length { get; set; }
        public ITag Tag { get; set; }

        int _position;

        public string TypeName()
        {
            return Tag.TagClass switch
            {
                // universal
                0 => Tag.TagNumber switch
                {
                    0x00 => "EOC",
                    0x01 => "BOOLEAN",
                    0x02 => "INTEGER",
                    0x03 => "BIT_STRING",
                    0x04 => "OCTET_STRING",
                    0x05 => "NULL",
                    0x06 => "OBJECT_IDENTIFIER",
                    0x07 => "ObjectDescriptor",
                    0x08 => "EXTERNAL",
                    0x09 => "REAL",
                    0x0A => "ENUMERATED",
                    0x0B => "EMBEDDED_PDV",
                    0x0C => "UTF8String",
                    0x10 => "SEQUENCE",
                    0x11 => "SET",
                    0x12 => "NumericString",
                    0x13 => "PrintableString",// ASCII subset
                    0x14 => "TeletexString",// aka T61String
                    0x15 => "VideotexString",
                    0x16 => "IA5String",// ASCII
                    0x17 => "UTCTime",
                    0x18 => "GeneralizedTime",
                    0x19 => "GraphicString",
                    0x1A => "VisibleString",// ASCII subset
                    0x1B => "GeneralString",
                    0x1C => "UniversalString",
                    0x1E => "BMPString",
                    _ => "Universal_" + Tag.TagNumber.ToString(),
                },
                1 => "Application_" + Tag.TagNumber.ToString(),
                2 => "[" + Tag.TagNumber.ToString() + "]",// Context
                3 => "Private_" + Tag.TagNumber.ToString(),
                _ => "",
            };
        }

        public string Content()
        {
            throw new NotImplementedException();
        }

        public int StartPosition()
        {
            return 0;
        }

        public int ContentPosition()
        {
            return HeaderBytesCount;
        }

        public int EndPosition()
        {
            return HeaderBytesCount + Math.Abs(Length);
        }

        public int LengthPosition()
        {
            return 1;
        }

        

        public byte Get()
        {
            return Data[++_position];
        }
    }
    
}
