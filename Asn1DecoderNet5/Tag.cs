using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;

namespace Asn1DecoderNet5.Tags
{
    /// <summary>
    /// Tag class
    /// </summary>
    public struct Tag : ITag
    {
        /// <summary>
        /// Creates new instance of Tag
        /// </summary>
        /// <param name="stream">Stream that is being currently decoded</param>
        /// <param name="streamPosition">Position in the stream</param>
        public Tag(byte[] stream, ref int streamPosition)
        {
            TagClass = stream[streamPosition] >> 6;
            TagNumber = stream[streamPosition];
            IsConstructed = ((stream[streamPosition] & 0x20) != 0);
            Childs = new();
            streamPosition++;

            TagName =
               TagClass switch
               {
                   0 =>
                        TagNumber switch
                        {
                            0 => "EOC",
                            1 => "BOOLEAN",
                            2 => "INTEGER",
                            3 => "BIT_STRING",
                            4 => "OCTET_STRING",
                            5 => "NULL",
                            6 => "OBJECT_IDENTIFIER",
                            7 => "ObjectDescriptor",
                            8 => "EXTERNAL",
                            9 => "REAL",
                            10 => "ENUMERATED",
                            11 => "EMBEDDED_PDV",
                            12 => "UTF8String",
                            48 => "SEQUENCE",
                            49 => "SET",
                            18 => "NumericString",
                            19 => "PrintableString", // ASCII subset
                            20 => "TeletexString", // aka T61String
                            21 => "VideotexString",
                            22 => "IA5String", // ASCII
                            23 => "UTCTime",
                            24 => "GeneralizedTime",
                            25 => "GraphicString",
                            26 => "VisibleString", // ASCII subset
                            27 => "GeneralString",
                            28 => "UniversalString",
                            30 => "BMPString",
                            _ => $"Universal_{TagNumber}"
                        },
                   1 => $"Application_{TagNumber & 0x1F}",
                   2 => $"[{TagNumber & 0x1F}]",
                   3 => $"Private_{TagNumber & 0x1F}",
                   _ => $"Uknown_{TagNumber & 0x1F}"
               };

            Content = null;
            ReadableContent = null;
        }
        public int TagNumber { get; set; }
        public string TagName { get; set; }
        public int TagClass { get; set; }
        public bool IsConstructed { get; }
        public bool IsUniversal => this.TagClass == 0x00;
        public bool IsEoc => TagClass == 0x00 && TagNumber == 0x00;
        public List<ITag> Childs { get; set; }
        public byte[] Content { get; set; }
        public string ReadableContent { get; set; }

        public void ConvertContentToReadableContent()
        {
            if (!IsUniversal)
            {
                if (Childs.Count < 1)
                {
                    ReadableContent = ParseOctetString();
                    return;
                }
                ReadableContent = "";
                return;
            }
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

            ReadableContent = (TagNumber & 0x1F) switch
            {
                0x01 => ParseBoolean(),
                0x02 => ParseInteger(),
                0x03 => ParseBitString(),
                0x04 => ParseOctetString(),
                0x05 => "",
                0x06 => ParseOid(),
                0x0A => ParseInteger(),
                0x10 or 0x11 => "",
                0x0C => ParseUtfString(),
                0x12 or 0x13 or 0x14 or 0x15 or 0x16 or 0x1A or 0x1B => ParseIsoString(),
                0x1E => ParseBmpString(),
                0x17 or 0x18 => ParseTime(),
            };
#pragma warning restore CS8509
        }

        string ParseOid()
        {
            string oid = ""; //final OID
            string buf = ""; //buffer for bytes larger than 128
            var hexValue = BitConverter.ToString(Content).Split("-").ToList();

            //convert the bytes to OID number
            for (int i = 0; i < hexValue.Count; i++)
            {
                string hex = hexValue[i];
                int num = Convert.ToInt32(hexValue[i], 16);
                if (i == 0) //decode first two numbers
                {
                    if (num.ToString().Length >= 2)
                    {
                        var x = num / 40;
                        var z = num % 40;
                        oid += $"{x}.{z}";
                    }
                    else
                    {
                        oid += $"0.{num}";
                    }
                }
                else
                {
                    if (num > 127)
                        buf += hexValue[i];

                    else if (num < 128 && buf != "")
                    {
                        buf += hexValue[i];
                        var binBuf = Convert.ToString(Convert.ToInt64(buf, 16), 2); //converts the buffer into binary
                        List<string> list = new List<string>();
                        List<string> _list = new List<string>();
                        for (int b = binBuf.Length - 8; b > -1; b -= 8) //split buffer by 8
                        {
                            list.Add(binBuf.Substring(b, 8));
                        }
                        list.Reverse();
                        for (int p = 0; p < list.Count; p++)
                        {
                            if (p == 0) //process first byte
                            {
                                string itm = list[0];
                                if (itm.Length < 8) //check if the byte is really 8 bits long, if not, lets make it that long by adding 0s
                                    itm = itm.PadLeft(8, '0');
                                if (itm[0] == '1') //if 8th bit is one, set it to 0
                                    itm = "0" + itm.Remove(0, 1);
                                _list.Add(itm.Remove(0, 1));
                            }
                            else
                            {
                                string itm = list[p];
                                _list.Add(itm.Remove(0, 1));
                            }
                        }

                        string last = ""; //the last bit
                        foreach (var item in _list)
                        {
                            last += item;
                        }

                        oid += $".{Convert.ToInt32(last, 2)}";
                        buf = "";
                    }
                    else
                    {
                        oid += $".{num}";
                    }
                }
            }

            //get the full OID description from OID list
            for (int i = 0; i < OID.OidList.GetLength(0); i++)
            {
                var items = OID.OidList;

                if (items[i, 0] == oid)
                {
                    oid = $"{items[i, 0]}, {items[i, 1]}{(items[i, 2] == "" ? "" : $", {items[i, 2]}")}{(items[i, 3] == "" ? "" : $", {items[i, 3]}")}";
                }
            }

            return oid;
        }

        string ParseUtfString()
        {
            StringBuilder sb = new();
            int Ex(byte c)
            {
                if (c < 0x80 || c > 0xC0)
                    throw new Exception($"Invalid UFT-8 continuation byte: {c}");
                return c & 0x3F;
            }

            string Surrogate(int cp)
            {
                if (cp < 0x10000)
                    throw new Exception($"UTF-8 overlong encoding, codepoint encoded in 4 bytes: {cp}");
                return char.ConvertFromUtf32(cp);
            }

            for (int i = 0; i < Content.Length;)
            {
                var c = Content[i++];
                if (c < 0x80)
                {
                    var _c =  Convert.ToChar(c);
                    sb.Append(_c);
                }
                else if (c < 0xC0)
                    throw new Exception($"Invalid UTF-8 starting byte: {c}");
                else if (c < 0xE0)
                {
                    var _c = Convert.ToChar(((c & 0x1F) << 6) | Ex(Content[i++]));
                    sb.Append(_c);
                }
                else if (c < 0xF0)
                {
                    var _c = Convert.ToChar(((c & 0x0F) << 12) | (Ex(Content[i++]) << 6) | Ex(Content[i++]));
                    sb.Append(_c);
                }
                else if (c < 248)
                {
                    var _c = Surrogate(((c & 0x07) << 18) | (Ex(Content[i++]) << 12) | (Ex(Content[i++]) << 6) | Ex(Content[i++]));
                    sb.Append(_c);
                }
                else
                    throw new Exception($"Invalid UTF-8 starting byte: {c}");
            }
            return sb.ToString();
        }

        string ParseIsoString()
        {
            StringBuilder sb = new();

            foreach (var c in Content)
            {
                sb.Append((char)c);
            }

            return sb.ToString();
        }

        string ParseBmpString()
        {
            StringBuilder sb = new();
            byte hi, low;
            for (int i = 0; i < Content.Length;)
            {
                hi = Content[i++];
                low = Content[i++];
                sb.Append((char)((hi << 8) | low));
            }

            return sb.ToString();
        }

        string ParseInteger()
        {
            StringBuilder sb = new();
            foreach (var hex in BitConverter.ToString(Content).Split("-").ToList())
            {
                sb.Append(hex);
            }
            return BigInteger.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber).ToString();
        }

        string ParseBitString()
        {
            byte unusedBits = Content[0];
            if (unusedBits > 7)
                throw new Exception($"Invalid BitString with unused bits {unusedBits}");

            int lengthBit = (Content.Length << 3) - unusedBits;
            StringBuilder sb = new();

            for (int i = 1; i < Content.Length; ++i)
            {
                byte b = Content[i];
                var skip = (i == (Content.Length - 1)) ? unusedBits : 0;

                for (int j = 7; j >= skip; --j)
                {
                    sb.Append(((b >> j) & 1) == 0 ? "0" : "1");
                }
            }

            return sb.ToString();
        }

        string ParseOctetString()
        {
            try
            {
                StringBuilder sb = new();
                char c;
                var s = ParseUtfString();
                for (int i = 0; i < s.Length; ++i)
                {
                    c = s[i];
                    if (c < 32 && c != 9 && c != 10 && c != 13)
                        throw new Exception("Unprintable character");
                    sb.Append(c);
                }
                return sb.ToString();
            }
            catch
            {

            }

            return BitConverter.ToString(Content).Replace("-", "");
        }

        string ParseTime()
        {
            // this only works for short year format, didn't bother by parsing long year format as I have never seen it being used in modern world
            var s = ParseIsoString();
            s = "20" + s;
            s = $"{s.Substring(0, 4)}/{s.Substring(4, 2)}/{s.Substring(6, 2)} {s.Substring(8, 2)}:{s.Substring(10, 2)}:{s.Substring(12, 2)} UTC";

            return s;
        }

        string ParseBoolean()
        {
            return (Content[0] == 0xff).ToString();
        }
    }
}
