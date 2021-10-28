using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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
            Childs = new List<ITag>();
            streamPosition++;

#if NET5_0_OR_GREATER
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
#else
            switch (TagClass)
            {
                case 0:
                    switch (TagNumber)
                    {
                        case 0:
                            TagName = "EOC";
                            break;
                        case 1:
                            TagName = "BOOLEAN";
                            break;
                        case 2:
                            TagName = "INTEGER";
                            break;
                        case 3:
                            TagName = "BIT_STRING";
                            break;
                        case 4:
                            TagName = "OCTET_STRING";
                            break;
                        case 5:
                            TagName = "NULL";
                            break;
                        case 6:
                            TagName = "OBJECT_IDENTIFIER";
                            break;
                        case 7:
                            TagName = "ObjectDescriptor";
                            break;
                        case 8:
                            TagName = "EXTERNAL";
                            break;
                        case 9:
                            TagName = "REAL";
                            break;
                        case 10:
                            TagName = "ENUMERATED";
                            break;
                        case 11:
                            TagName = "EMBEDDED_PDV";
                            break;
                        case 12:
                            TagName = "UTF8String";
                            break;
                        case 48:
                            TagName = "SEQUENCE";
                            break;
                        case 49:
                            TagName = "SET";
                            break;
                        case 18:
                            TagName = "NumericString";
                            break;
                        case 19:
                            TagName = "PrintableString";
                            break;
                        case 20:
                            TagName = "TeletexString";
                            break;
                        case 21:
                            TagName = "VideotexString";
                            break;
                        case 22:
                            TagName = "IA5String";
                            break;
                        case 23:
                            TagName = "UTCTime";
                            break;
                        case 24:
                            TagName = "GeneralizedTime";
                            break;
                        case 25:
                            TagName = "GraphicString";
                            break;
                        case 26:
                            TagName = "VisibleString";
                            break;
                        case 27:
                            TagName = "GeneralString";
                            break;
                        case 28:
                            TagName = "UniversalString";
                            break;
                        case 30:
                            TagName = "BMPString";
                            break;
                        default:
                            TagName = $"Universal_{TagNumber}";
                            break;
                    }
                    break;
                case 1:
                    TagName = $"Application_{TagNumber & 0x1F}";
                    break;
                case 2:
                    TagName = $"[{TagNumber & 0x1F}]";
                    break;
                case 3:
                    TagName = $"Private_{TagNumber & 0x1F}";
                    break;
                default:
                    TagName = $"Uknown_{TagNumber & 0x1F}";
                    break;
            }
#endif
            Content = null;
            ReadableContent = null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int TagNumber { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int TagClass { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsConstructed { get; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsUniversal => this.TagClass == 0x00;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsEoc => TagClass == 0x00 && TagNumber == 0x00;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<ITag> Childs { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ReadableContent { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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
#if NET5_0_OR_GREATER
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
#else
            switch (TagNumber & 0x1F)
            {
                case 0x01: ReadableContent = ParseBoolean(); break;
                case 0x02: ReadableContent = ParseInteger(); break;
                case 0x03: ReadableContent = ParseBitString(); break;
                case 0x04: ReadableContent = ParseOctetString(); break;
                case 0x05: ReadableContent = ""; break;
                case 0x06: ReadableContent = ParseOid(); break;
                case 0x0A: ReadableContent = ParseInteger(); break;
                case 0x10: ReadableContent = ""; break;
                case 0x11: ReadableContent = ""; break;
                case 0x0C: ReadableContent = ParseUtfString(); break;
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x15:
                case 0x16:
                case 0x1A:
                case 0x1B:
                    ReadableContent = ParseIsoString();
                    break;
                case 0x1E: ReadableContent = ParseBmpString(); break;
                case 0x17: ReadableContent = ParseTime(); break;
                default:
                    break;
            }
#endif
#pragma warning restore CS8509
        }

        string ParseOid()
        {
            var hexValue = BitConverter.ToString(Content).Split('-').ToList();

            var oid = ConvertBytesToOidString(hexValue);

            var items = OID.OidList;

            //get the full OID description from OID list
            for (int i = 0; i < OID.OidList.GetLength(0); i++)
            {
                if (items[i, 0] == oid)
                {
                    oid = $"{items[i, 0]}, {items[i, 1]}{(items[i, 2] == "" ? "" : $", {items[i, 2]}")}{(items[i, 3] == "" ? "" : $", {items[i, 3]}")}";
                }
            }

            return oid;
        }

        private static string ConvertBytesToOidString(List<string> hexValue)
        {
            var oid = "";
            var buf = "";

            var firstByte = Convert.ToInt32(hexValue[0], 16);
            if (firstByte.ToString().Length >= 2)
            {
                var x = firstByte / 40;
                var z = firstByte % 40;
                oid += $"{x}.{z}";
            }
            else
            {
                oid += $"0.{firstByte}";
            }
            for (int i = 1; i < hexValue.Count; i++)
            {
                int num = Convert.ToInt32(hexValue[i], 16);

                if (num > 127)
                {
                    buf += hexValue[i];
                    continue;
                }

                if (num < 128 && buf != "")
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
                        if (p != 0) //process first byte
                        {
                            string itm = list[0];
                            if (itm.Length < 8) //check if the byte is really 8 bits long, if not, lets make it that long by adding 0s
                                itm = itm.PadLeft(8, '0');
                            if (itm[0] == '1') //if 8th bit is 1, set it to 0
                                itm = "0" + itm.Remove(0, 1);
                            _list.Add(itm.Remove(0, 1));
                        }
                        else
                        {
                            string itm = list[p];
                            _list.Add(itm.Remove(0, 1));
                        }
                    }

                    string last = "";
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

            return oid;
        }

        string ParseUtfString()
        {
            StringBuilder sb = new StringBuilder();
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
                    var _c = Convert.ToChar(c);
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
            StringBuilder sb = new StringBuilder();

            foreach (var c in Content)
            {
                sb.Append((char)c);
            }

            return sb.ToString();
        }

        string ParseBmpString()
        {
            StringBuilder sb = new StringBuilder();
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
            StringBuilder sb = new StringBuilder();
            foreach (var hex in BitConverter.ToString(Content).Split('-').ToList())
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

            StringBuilder sb = new StringBuilder();

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
                StringBuilder sb = new StringBuilder();
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
