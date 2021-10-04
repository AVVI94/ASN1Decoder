using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;

namespace Asn1DecoderNet5
{
    /// <summary>
    /// ASN1 Decoder
    /// </summary>
    public sealed class Decoder
    {
        internal Decoder()
        {
        }

        #region privateMethods
        internal ITag Decode_(byte[] data)
        {
            var tag = new Tag(data, ref i);
            var len = DecodeLength(data);
            var start = i;

            if (tag.IsConstructed)
            {
                GetChilds();
            }
            else if (tag.IsUniversal && (tag.TagNumber == 0x03 || tag.TagNumber == 0x04))
            {
                try
                {
                    if (tag.TagNumber == 0x03)
                        if (data[++i] != 0)
                            throw new Exception("BitString with unused bits cannot encapsulate");
                    GetChilds();
                    foreach (var ch in tag.Childs)
                    {
                        if (ch.IsEoc)
                            throw new Exception("EOC is not supposed to be actual content");
                    }
                }
                catch
                {
                    tag.Childs.Clear();
                }
            }

            if (tag.Childs.Count == 0)
            {
                if (len == null)
                    throw new Exception($"Cannot skip over an invalid tag with indefinite length at offset {start}");

                tag.Content = data[start..(len.Value + start)];
                i = start + Math.Abs(len.Value);
            }
            return tag;

            void GetChilds()
            {
                if (len != null)
                {
                    var end = start + len;
                    if (end > data.Length)
                        throw new IndexOutOfRangeException($"Container at offset {start} has a length of {len}, which is past the end of the stream");

                    while (i < end)
                    {
                        tag.Childs.Add(Decode_(data));
                    }
                    if (i != end)
                        throw new IndexOutOfRangeException($"Content size is not correct for container at offset {start}");
                }
                else
                {
                    try
                    {
                        for (; ; )
                        {
                            var child = Decode_(data);
                            if (child.IsEoc)
                                break;
                            tag.Childs.Add(child);
                        }
                        len = start - i;
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Exception while decoding undefined length content at offset {start}", e);
                    }
                }
            }
        }

        int? DecodeLength(byte[] data)
        {
            var buf = (int)data[i++];
            var len = buf & 0x7f;

            if (len == buf)
                return len;
            if (len == 0)
                return null;
            if (len > 6)
                throw new ArgumentOutOfRangeException("Length over 48 bits not supported;");

            buf = 0;
            for (int y = 0; y < len; y++)
            {
                buf = (buf * 256) + data[i++];
            }

            return buf;
        }
        static void ProcessKeyUsage(ITag tag, string tmp)
        {
            var hex = BitConverter.ToString(tag.Content);
            string bin = "";
            foreach (var item in hex.Split("-"))
            {
                bin += Convert.ToString(Convert.ToInt64(item, 16), 2);
            }

            string ku = "";
            bin = bin.PadRight(8, '0');
            bin = bin.Substring(bin.Length - 8);
            for (int i = 0; i < bin.Length; i++)
            {
                string _tmp = bin.Substring(i, 1);
                if (Enum.IsDefined(typeof(KU), i) && _tmp == "1")
                {
                    ku += $"{(KU)i}, ";
                }
                if (tmp == "00000000")
                    ku = "decipherOnly";
            }
            tag.ReadableContent = ku;
        }
        #endregion

        #region privateFields
        //"global" stream position
        int i = 0;

        static string _lastOid;
        #endregion

        #region publicAPI
        /// <summary>
        /// Decode the DER encoded data sequence
        /// </summary>
        /// <param name="data">DER encoded data represented in bytes</param>
        /// <returns><see cref="ITag"/> containing the decoded sequence</returns>
        public static ITag Decode(byte[] data)
        {
            Decoder dc = new();
            return dc.Decode_(data);
        }

        /// <summary>
        /// Converts Tag (and all its childs) into readable string
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <param name="structureSpacer">String used for structurizing the ASN1 output, " | " is recomended (whit the whitespaces)</param>
        /// <param name="maxContentLineLength">How many characters can one line have</param>
        /// <returns>Formated ASN1 structure string</returns>
        public static string TagToString(ITag tag, string structureSpacer, int maxContentLineLength)
        {
            ConvertTagsContents(tag);
            return TagToStringRecurse(tag, 0);

            #region localFunctions
            string MultiplyString(string to, int multiplier)
            {
                string tmp = "";
                for (int i = 0; i < multiplier; i++)
                {
                    tmp += to;
                }
                return tmp;
            }

            string TagToStringRecurse(ITag tag, int lvl)
            {
                string tmp = "";
                if (tag.Childs.Count > 0)
                {
                    tmp += $"{MultiplyString(structureSpacer, lvl)}{tag.TagName}{Environment.NewLine}";
                    foreach (var child in tag.Childs)
                    {
                        tmp += TagToStringRecurse(child, lvl + 1);
                    }
                }
                else
                {
                    if (tag.TagNumber == 6)
                    {
                        _lastOid = tag.ReadableContent;
                    }
                    #region OID_SpecificProcessing
                    if (tag.TagNumber == 3 && _lastOid == "2.5.29.15, keyUsage, X.509 extension")
                    {
                        ProcessKeyUsage(tag, tmp);
                    }
                    #endregion
                    if (tag.ReadableContent.Length > maxContentLineLength)
                    {
                        var spacer = MultiplyString(structureSpacer, lvl + 1);
                        var firstSplit = tag.ReadableContent.Replace("\r\n", "\n").Split("\n");

                        for (int y = 0; y < firstSplit.Length; y++)
                        {

                            var split = firstSplit[y].Select((c, index) => new { c, index })
                                                       .GroupBy(x => x.index / maxContentLineLength)
                                                       .Select(group => group.Select(elem => elem.c))
                                                       .Select(chars => new string(chars.ToArray())).ToArray();

                            for (int i = 0; i < split.Length; i++)
                            {
                                split[i] = spacer + split[i];
                            }
                            firstSplit[y] = string.Join(Environment.NewLine, split);
                        }

                        tag.ReadableContent = Environment.NewLine;
                        tag.ReadableContent += string.Join(Environment.NewLine, firstSplit);

                        tag.ReadableContent = tag.ReadableContent.TrimEnd();

                        tmp += $"{MultiplyString(structureSpacer, lvl)}{tag.TagName} {tag.ReadableContent}{Environment.NewLine}";
                    }
                    else
                        tmp += $"{MultiplyString(structureSpacer, lvl)}{tag.TagName} {tag.ReadableContent}{Environment.NewLine}";
                }

                return tmp;
            }

            void ConvertTagsContents(ITag tag)
            {
                foreach (var child in tag.Childs)
                {
                    if (child.Childs.Count > 0)
                        ConvertTagsContents(child);
                    else
                        child.ConvertContentToReadableContent();
                }
            }
            #endregion
        }
        #endregion
    }
}
