using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;
#nullable enable
namespace Asn1DecoderNet5.old
{
    public class Decoder
    {
        /// <summary>
        /// Decodes the length of containers content from byte array
        /// </summary>
        /// <param name="stream">the data to decode</param>
        /// <returns>
        /// <para><see cref="length"/> the length in decimal format</para>
        /// <para><see cref="lengthBytes"/> count of the length bytes from the TLV triplet</para></returns>
        public (int length, int? lengthBytes) DecodeLength(byte[] stream)
        {
            if (stream[1] < 128)
                return (stream[1], 1);
            if (stream[1] == 128)
                return (0, null);
            if (stream[1] == 129)
                return (stream[2], 2);
            if (stream[1] > 130)
                throw new ArgumentOutOfRangeException("Length over 48 bits not supported;");

            var hex = BitConverter.ToString(stream).Split('-');
            var str = hex[2] + hex[3];
            return (int.Parse(str, System.Globalization.NumberStyles.HexNumber), 3);
        }

        int decodeCalls = 0;
        int offsetInTheMainStream = 0;
        /// <summary>
        /// Decode the whole sequence (the tag and its childerns)
        /// </summary>
        /// <param name="stream">the sequence to decode</param>
        /// <returns><see cref="ITag"/> Top tag containg the sequence in its <see cref="ITag.Childs"/> property,
        /// if the Childs list is empty then tag has a somewhat printable content (or at least it should be printable)</returns>
        public ITag DecodeWholeSequenceRecursively(byte[] stream)
        {
            decodeCalls++;
            if (stream[0] == 0 && stream[1] == 0)
            {
                stream = stream[2..^0];
            }
            (int len, int? lenBytes) = DecodeLength(stream);

            //if the length is indefinite, we gonna modify the stream for the content termination bytes (for the double 0 bytes)
            if (lenBytes == null)
            {
                throw new NotSupportedException("Containers with indefinite length are currently not supported");
                //(int length, byte[] editedStream) = GetIndefiniteLengthAndModifyTheArray(stream);
                //len = length;
            }

            var tag = new Tag(stream, len, lenBytes ?? 1);
            if (tag == null)
                tag = new Tag(stream, len, lenBytes ?? 1);

            byte[] subStream = new byte[tag.Content.Length];
            Array.Copy(stream, tag.LengthBytesCount + 1, subStream, 0, tag.Content.Length);

            #region funcions
            //Can be called only AFTER Decode method local variables are declarated and initialized!!!
            void GetChilds()
            {
                if (len != 0)
                {
                    var end = subStream.Length;
                    var inLoopSubStreamPosition = 0;
                    var outLoopSubStreamPosition = 0;

                    if (end > stream.Length)
                        throw new IndexOutOfRangeException($"Container at offset {offsetInTheMainStream} has a length of {len} which is past its parent container length");

                    while (outLoopSubStreamPosition < end)
                    {
#nullable disable
                        tag.Childs.Add(this.DecodeWholeSequenceRecursively(subStream));
                        inLoopSubStreamPosition = tag.Childs[^1].Length;
                        outLoopSubStreamPosition += tag.Childs[^1].Length;
                        offsetInTheMainStream += inLoopSubStreamPosition;
#nullable restore           
                        if (inLoopSubStreamPosition < subStream.Length)
                        {
                            subStream = subStream.Skip(inLoopSubStreamPosition).ToArray();
                        }
                    }

                    if (outLoopSubStreamPosition != end)
                    {
                        var dcs = decodeCalls.ToString();
                        dcs += dcs.EndsWith("1") ? "st" : dcs.EndsWith("2") ? "nd" : dcs.EndsWith("3") ? "rd" : "th";
                        throw new IndexOutOfRangeException($"Content size for container at offset {offsetInTheMainStream} is not valid");
                    }
                }
                else
                {
                    try
                    {
                        for (; ; )
                        {
                            throw new Exception();

                            if (subStream[1..^0].Length == 0)
                                break;
                            var s = DecodeWholeSequenceRecursively(subStream[1..^0]);
                            if (s.IsEoc())
                                break;
                            tag.Childs.Add(s);
                        }
                        //(int length, byte[] editedStream) = GetIndefiniteLengthAndModifyTheArray(stream);
                    }
                    catch
                    {
                        throw new NotSupportedException("Containers with indefinite length are currently not supported");
                        throw new Exception($"Exception while decoding undefined length content for container at offset {offsetInTheMainStream}");
                    }
                }
            }
            #endregion

            if (tag.IsConstructed)
            {
                GetChilds();
            }
            else if (tag.IsUniversal() && ((tag.TagNumber == 0x03) || (tag.TagNumber == 0x04)))
            {
                try
                {
                    if (tag.TagNumber == 0x03 && stream[(lenBytes ?? 1) + 1] != 0)
                        throw new Exception("BitString with unused bits cannot encapsulate");
                    GetChilds();
                    foreach (var item in tag.Childs)
                    {
                        if (item.IsEoc())
                            throw new Exception($"EOC is not supported here, offset {offsetInTheMainStream}");
                    }
                }
                catch
                {
                    tag.Childs.Clear();
                }
            }
            if (tag.Childs.Count == 0)
            {
                if (len == 0 && tag.TagNumber != 5)
                    throw new Exception("Cannot skip over invalid tag with undefined length");

            }
            offsetInTheMainStream += 1 + lenBytes ?? 1;
            return tag;
        }

        (int length, byte[] editedStream) GetIndefiniteLengthAndModifyTheArray(byte[] stream, bool canModifyTheArray = true)
        {
            int i;
            bool firstZero = false;
            for (i = stream.Length - 1; i > 1; i--)
            {
                if (stream[i] == 0)
                {
                    if (stream[i] == 0 && firstZero)
                    {
                        break;
                    }
                    firstZero = true;
                }
                else
                    firstZero = false;
            }
            byte[] newStream = null;

            if (canModifyTheArray)
            {
                //since the length is indefinite, we just remove the double 0s bytes
                newStream = (stream[0..i]).Concat(stream[(i + 1)..^1]).ToArray();
                i -= 2;
            }

            return (i, newStream);
        }

        public List<ITag> Decode(byte[] fullStream)
        {
            List<ITag> list = new();
            for (int i = 0; i < fullStream.Length;)
            {
                (int length, int? lenBytesCount) = DecodeLength(fullStream);

                //if the length is indefinite, we gonna modify the stream for the content termination bytes (for the double 0 bytes)
                if (lenBytesCount == null)
                {
                    throw new NotSupportedException("Containers with indefinite length are currently not supported");
                    (int len, byte[] editedStream) = GetIndefiniteLengthAndModifyTheArray(fullStream, false);
                    length = len;
                }

                list.Add(DecodeWholeSequenceRecursively(fullStream[i..(length + (lenBytesCount ?? 1) + 1)]));
                i += length + (lenBytesCount ?? 1) + 1;
            }
            return list;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Convert HEX string to human readable string
        /// </summary>
        /// <param name="hexInput"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ConvertHexToString(string hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }

        public static string ParseUtfString(byte[] content)
        {
            
        }
    }
}
