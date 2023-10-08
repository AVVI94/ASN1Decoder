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
#pragma warning disable IDE1006 // Naming styles
        //"global" stream position
        int i = 0;
#pragma warning restore IDE1006 // Naming styles
        public ITag Decode(byte[] data)
        {
            var tag = new Tag(data, ref i);
            var len = DecodeLength(data);
            var start = i;

            if (tag.IsConstructed)
            {
                GetChilds(data, ref tag, ref len, start);
            }
            else if (tag.IsUniversal && (tag.TagNumber == (int)Tags.Tags.BIT_STRING || tag.TagNumber == (int)Tags.Tags.OCTET_STRING))
            {
                try
                {
                    if (tag.TagNumber == (int)Tags.Tags.BIT_STRING)
                        if (data[++i] != 0)
                            throw new Exception("BitString with unused bits cannot encapsulate");
                    GetChilds(data, ref tag, ref len, start);
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
                Array.Copy(data, start, tag.Content = new byte[len.Value], 0, len.Value);
                i = start + Math.Abs(len.Value);
            }
            //check for KeyUsage OID in first child tag
            else if (tag.IsKeyUsageSequence())
            {
                //parse out from child tags if the KU is marked as critical and get the BIT_STRING KU tag itself
                var crit = tag.IsKeyUsageCritical();
                var bs = tag.GetKeyUsageBitStringTag();
                //change the BIT_STRING tag instance from generic Tag to KeyUsageTag
                tag.Childs[2].Childs[0] = new KeyUsageTag(in bs, crit);
            }
            tag.IsConstructed = tag.Childs.Count > 0;
            return tag;
        }

        void GetChilds(byte[] data, ref Tag tag, ref int? len, int start)
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
                    while (true)
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
