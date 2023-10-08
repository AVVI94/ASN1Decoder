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
        public bool IsConstructed { get; internal set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public readonly bool IsUniversal => this.TagClass == 0x00;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public readonly bool IsEoc => TagClass == 0x00 && TagNumber == 0x00;
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
            ReadableContent = (TagNumber & 0x1F) switch
            {
                0x01 => ParseBoolean(),
                0x02 => ParseInteger(),
                0x03 => ParseBitString(Content),
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

        readonly string ParseOid()
        {
            var oid = Encoding.OidEncoding.GetString(Content);

            var retVal = OID.OidDictionary.TryGetValue(oid, out var _oid);
            if (retVal)
            {
                oid = _oid.Value;
                if (!string.IsNullOrWhiteSpace(_oid.FriendlyName))
                    oid += $", {_oid.FriendlyName}";
                if (!string.IsNullOrWhiteSpace(_oid.Comment))
                    oid += $", {_oid.Comment}";
                //oid = $"{_oid.Value}, {_oid.FriendlyName}, {_oid.Comment}";
            }

            return oid;
        }

        readonly string ParseUtfString()
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

        readonly string ParseIsoString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var c in Content)
            {
                sb.Append((char)c);
            }

            return sb.ToString();
        }

        readonly string ParseBmpString()
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

        readonly string ParseInteger()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var hex in BitConverter.ToString(Content).Split('-').ToList())
            {
                sb.Append(hex);
            }
            return BigInteger.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber).ToString();
        }

        internal static string ParseBitString(byte[] data)
        {
            byte unusedBits = data[0];
            if (unusedBits > 7)
                throw new Exception($"Invalid BitString with unused bits {unusedBits}");

            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < data.Length; ++i)
            {
                byte b = data[i];
                var skip = (i == (data.Length - 1)) ? unusedBits : 0;

                for (int j = 7; j >= skip; --j)
                {
                    sb.Append(((b >> j) & 1) == 0 ? "0" : "1");
                }
            }

            return sb.ToString();
        }

        readonly string ParseOctetString()
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

        readonly string ParseTime()
        {
            var s = ParseIsoString();
            if (this.TagNumber == 23)
                s = "20" + s;
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator
            s = $"{s.Substring(0, 4)}/{s.Substring(4, 2)}/{s.Substring(6, 2)} {s.Substring(8, 2)}:{s.Substring(10, 2)}:{s.Substring(12, 2)} UTC";
#pragma warning restore IDE0057 // Use range operator
#pragma warning restore IDE0079 // Remove unnecessary suppression

            return s;
        }

        readonly string ParseBoolean()
        {
            return (Content[0] == 0xff).ToString();
        }
    }
}
