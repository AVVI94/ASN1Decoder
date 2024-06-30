using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASN1Decoder.NET.Tags
{
    /// <summary>
    /// Decoded tags in decimal format
    /// </summary>
    public enum TagNames
    {
        //EOC = 0,
        BOOLEAN = 1,
        INTEGER = 2,
        BIT_STRING = 3,
        OCTET_STRING = 4,
        NULL = 5,
        OBJECT_IDENTIFIER = 6,
        ObjectDescriptor = 7,
        EXTERNAL = 8,
        REAL = 9,
        ENUMERATED = 10,
        EMBEDDED_PDV = 11,
        UTF8String = 12,
        SEQUENCE = 48,
        SET = 49,
        NumericString = 18,
        PrintableString = 19,
        TeletexString = 20,
        VideotexString = 21,
        IA5String = 22,
        UTCTime = 23,
        GeneralizedTime = 24,
        GraphicString = 25,
        VisibleString = 26,
        GeneralString = 27,
        UniversalString = 28,
        BMPString = 30        
    }
    internal enum KU
    {
        digitalSignature = 0,
        nonRepudiation = 1,
        keyEncipherment = 2,
        dataEncipherment = 3,
        keyAgreement = 4,
        keyCertSign = 5,
        cRLSign = 6,
        encipherOnly = 7
    }
}
