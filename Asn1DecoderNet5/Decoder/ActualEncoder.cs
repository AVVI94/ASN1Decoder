using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asn1DecoderNet5.Interfaces;
namespace Asn1DecoderNet5;
internal class ActualEncoder
{
    internal static List<byte> Encode(IReadOnlyTag tag)
    {
        var res = new List<byte>();
        var res2 = new List<byte>();
        res.Add((byte)tag.TagNumber);
        if (tag.Childs.Count > 0)
        {
            foreach (var child in tag.Childs)
            {
                res2.AddRange(Encode(child));
            }
        }
        else
        {
            res2.AddRange(tag.Content);
        }
        res.AddRange(EncodeLength(res2.Count));
        res.AddRange(res2);
        return res;
    }

    internal static byte[] EncodeLength(int length)
    {
        if (length < 128)
            return new[] { (byte)length };
        var r = BitConverter.GetBytes(length);
        int l = 0;
        while (r[++l] != 0) ;
        var res = new byte[l + 1];
        res[l] = (byte)(l + 128);
        Array.Copy(r, res, l);
        Array.Reverse(res);
        return res;
    }
}
