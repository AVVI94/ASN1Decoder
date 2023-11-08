using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asn1DecoderNet5.OIDEncoding
{
    /// <summary>
    /// Class for help with Encoding/Decoding the OID values
    /// </summary>
    public static class OidEncoding
    {
        public static string GetString(byte[] oid)
        {
            if (oid is null || oid.Length == 0)
            {
                throw new ArgumentNullException(nameof(oid));
            }

            var hexValue = BitConverter.ToString(oid).Split('-').ToList();

            var oidString = ConvertBytesToOidString(hexValue);

            return oidString;
        }
        public static byte[] GetBytes(string oid)
        {
            if (string.IsNullOrWhiteSpace(oid))
            {
                throw new ArgumentException($"'{nameof(oid)}' cannot be null or whitespace.", nameof(oid));
            }
            //https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-object-identifier?redirectedfrom=MSDN
            var split = oid.Split('.');
            var int1 = char.Parse(split[0]) switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                _ => throw new ArgumentException("Invalid OID value at oid.Split('.') index 0"),
            };
            int int2 = int.Parse(split[1]);

            List<byte> res = new List<byte>
            {
                Convert.ToByte((40 * int1) + int2)
            };

            for (int i = 2; i < split.Length; i++)
            {
                var x = long.Parse(split[i]);

                if (x > 127)
                {
                    //https://www.sysadmins.lv/blog-en/how-to-encode-object-identifier-to-an-asn1-der-encoded-string.aspx
                    var bytes = BitConverter.GetBytes(x).Where(y => y != 0).Reverse().ToList();
                    var b = new byte[bytes.Count];
                    b[0] = (byte)(x & 0x7F);
                    for (int y = 1; y < bytes.Count; y++)
                        b[y] = (byte)(((x >> 7 * y)) | 0x80);

                    res.AddRange(b.Reverse());
                    continue;
                }

                res.Add(Convert.ToByte(split[i]));
            }
            return res.ToArray();
        }

        private static string ConvertBytesToOidString(List<string> hexValue)
        {
            string oid = "";
            string buff = "";

            int int32 = Convert.ToInt32(hexValue[0], 16);
            if (int32.ToString().Length >= 2)
            {
                int num1 = int32 / 40;
                int num2 = int32 % 40;
                oid += string.Format("{0}.{1}", (object)num1, (object)num2);
            }
            else
                oid += string.Format("0.{0}", (object)int32);

            for (int index1 = 1; index1 < hexValue.Count; ++index1)
            {
                //string str3 = hexValue[index1];
                int32 = Convert.ToInt32(hexValue[index1], 16);

                if (int32 > (int)sbyte.MaxValue)
                    buff += hexValue[index1];
                else if (int32 < 128 && buff != "")
                {
                    string binaryBufferString = Convert.ToString(Convert.ToInt64(buff + hexValue[index1], 16), 2);
                    List<string> binaryByteBuffer = new List<string>();
                    List<string> longBuffer = new List<string>();
                    for (int startIndex = binaryBufferString.Length - 8; startIndex > -1; startIndex -= 8)
                        binaryByteBuffer.Add(binaryBufferString.Substring(startIndex, 8));
                    binaryByteBuffer.Reverse();

                    string str5 = binaryByteBuffer[0];
                    if (str5.Length < 8)
                        str5 = str5.PadLeft(8, '0');
                    if (str5[0] == '1')
                        str5 = "0" + str5.Remove(0, 1);
                    longBuffer.Add(str5.Remove(0, 1));

                    for (int pos = 1; pos < binaryByteBuffer.Count; ++pos)
                    {
                        string str6 = binaryByteBuffer[pos];
                        longBuffer.Add(str6.Remove(0, 1));
                    }

                    string str7 = "";
                    foreach (string str8 in longBuffer)
                        str7 += str8;
                    oid += string.Format(".{0}", (object)Convert.ToInt32(str7, 2));
                    buff = "";
                }
                else
                    oid += string.Format(".{0}", (object)int32);
            }

            return oid;
        }
    }
}
