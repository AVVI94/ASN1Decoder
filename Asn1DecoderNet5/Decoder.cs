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
    public static class Decoder
    {
        #region privateMethods    
        private static void ConvertTagsContentsToReadableStringsRecurse(ITag _tag)
        {
            foreach (var child in _tag.Childs)
            {
                if (child.Childs.Count > 0)
                    ConvertTagsContentsToReadableStringsRecurse(child);
                else
                    child.ConvertContentToReadableContent();
            }
        }

        private static string TagToStringRecurse(ITag _tag, int lvl, string structureSpacer, int maxContentLineLength)
        {
            string tmp = "";
            if (_tag.Childs.Count > 0)
            {
                tmp += $"{MultiplyString(structureSpacer, lvl)}{_tag.TagName}{Environment.NewLine}";
                foreach (var child in _tag.Childs)
                {
                    tmp += TagToStringRecurse(child, lvl + 1, structureSpacer, maxContentLineLength);
                }
            }
            else
            {
                if (_tag.TagNumber == 6)
                {
                    _lastOid = _tag.ReadableContent;
                }
                #region OID_SpecificProcessing
                if (_tag.TagNumber == 3 && _lastOid == "2.5.29.15, keyUsage, X.509 extension")
                {
                    ConvertKeyUsageFromBitStringToReadableString(_tag, tmp);
                }
                #endregion
                if (maxContentLineLength > 0 && _tag.ReadableContent.Length > maxContentLineLength)
                {
                    string[] firstSplit = FormatContentByMaxCharactersPerLine(_tag, lvl, structureSpacer, maxContentLineLength);

                    _tag.ReadableContent = Environment.NewLine;
                    _tag.ReadableContent += string.Join(Environment.NewLine, firstSplit);

                    _tag.ReadableContent = _tag.ReadableContent.TrimEnd();

                    tmp += $"{MultiplyString(structureSpacer, lvl)}{_tag.TagName} {_tag.ReadableContent}{Environment.NewLine}";
                }
                else
                    tmp += $"{MultiplyString(structureSpacer, lvl)}{_tag.TagName} {_tag.ReadableContent}{Environment.NewLine}";
            }

            return tmp;
        }

        private static string[] FormatContentByMaxCharactersPerLine(ITag _tag, int lvl, string structureSpacer, int maxContentLineLength)
        {
            var spacer = MultiplyString(structureSpacer, lvl + 1);
            var firstSplit = _tag.ReadableContent.Replace("\r\n", "\n").Split('\n');

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

            return firstSplit;
        }

        private static string MultiplyString(string to, int multiplier)
        {
            string tmp = "";
            for (int i = 0; i < multiplier; i++)
            {
                tmp += to;
            }
            return tmp;
        }

        private static void ConvertKeyUsageFromBitStringToReadableString(ITag tag, string tmp)
        {
            var hex = BitConverter.ToString(tag.Content);
            string bin = "";
            foreach (var item in hex.Split('-'))
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
            return new ActualDecoder().Decode(data);
        }

        /// <summary>
        /// Converts Tag (and all its childs) into a readable string
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <param name="structureSpacer">String used for structurizing the ASN1 output, " | " is recomended (whit the whitespaces)</param>
        /// <param name="maxContentLineLength">How many content characters can be in single line, zero for unlimited</param>
        /// <returns>Formated ASN1 structure string</returns>
        public static string TagToString(ITag tag, string structureSpacer, int maxContentLineLength)
        {
            ConvertTagsContentsToReadableStringsRecurse(tag);
            return TagToStringRecurse(tag, 0, structureSpacer, maxContentLineLength);
        }
        
        #endregion
    }
}
