using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;

namespace Asn1DecoderNet5
{
    public class Decoder
    {
        internal Decoder()
        {
        }

        int i = 0;
        internal ITag Decode_(byte[] data)
        {
            int thisStart = i;
            var tag = new Tag(data, ref i);
            int tagLen = i - thisStart;
            var len = DecodeLength(data);
            var start = i;
            var header = start - thisStart;

            if (tag.IsConstructed)
            {
                GetChilds();
            }
            else if (tag.IsUniversal() && (tag.TagNumber == 0x03 || tag.TagNumber == 0x04))
            {
                try
                {
                    if (tag.TagNumber == 0x03)
                        if (data[++i] != 0)
                            throw new Exception("BitString with unused bits cannot encapsulate");
                    GetChilds();
                    foreach (var ch in tag.Childs)
                    {
                        if (ch.IsEoc())
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
                            if (child.IsEoc())
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

        public static ITag Decode(byte[] data)
        {
            Decoder dc = new();
            return dc.Decode_(data);
        }

        static string _lastOid;
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
                    if (tag.TagNumber == 3 && _lastOid == "2.5.29.15, keyUsage, X.509 extension")
                    {
                        ProcessKeyUsage(tag, tmp);
                    }
                    if (tag.ReadableContent.Length > maxContentLineLength)
                    {
                        var spacer = MultiplyString(structureSpacer, lvl + 1);
                        
                        var tmpList = new List<string>();
                        for (int p = 0; p < tag.ReadableContent.Length;) 
                        {
                            tmpList.Add(spacer + tag.ReadableContent.Substring(p, p + 63 < tag.ReadableContent.Length ? 63 : tag.ReadableContent.Length - p)); 
                            p += maxContentLineLength;
                        }
                        tag.ReadableContent = Environment.NewLine;
                        tag.ReadableContent += string.Join(Environment.NewLine, tmpList);
                        
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
    }
}
