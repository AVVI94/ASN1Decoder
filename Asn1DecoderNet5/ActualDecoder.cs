using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;

namespace Asn1DecoderNet5
{
    internal class ActualDecoder
    {
        //"global" stream position
        int i = 0;
        public ITag Decode(byte[] data)
        {
            var tag = new Tag(data, ref i);
            var len = DecodeLength(data);
            var start = i;

            if (tag.IsConstructed)
            {
                GetChilds(data, tag, ref len, start);
            }
            else if (tag.IsUniversal && (tag.TagNumber == 0x03 || tag.TagNumber == 0x04))
            {
                try
                {
                    if (tag.TagNumber == 0x03)
                        if (data[++i] != 0)
                            throw new Exception("BitString with unused bits cannot encapsulate");
                    GetChilds(data, tag, ref len, start);
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
#if NET5_0_OR_GREATER
                tag.Content = data[start..(len.Value + start)];
#else
                Array.Copy(data, start, tag.Content = new byte[len.Value], 0, len.Value);
#endif
                i = start + Math.Abs(len.Value);
            }
            return tag;
        }

        void GetChilds(byte[] data, Tag tag, ref int? len, int start)
        {
            if (len != null)
            {
                var end = start + len;
                if (end > data.Length)
                    throw new IndexOutOfRangeException($"Container at offset {start} has a length of {len}, which is past the end of the stream");

                while (i < end)
                {
                    tag.Childs.Add(Decode(data));
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
                        var child = Decode(data);
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

        int? DecodeLength(byte[] data)
        {
            var buf = (int)data[i++];
            var len = buf & 0x7f;

            if (len == buf)
                return len;
            if (len == 0)
                return null;
            if (len > 6)
                throw new ArgumentOutOfRangeException(nameof(data), "Length over 48 bits not supported;");

            buf = 0;
            for (int y = 0; y < len; y++)
            {
                buf = (buf * 256) + data[i++];
            }

            return buf;
        }
    }
}
