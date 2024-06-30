using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASN1Decoder.NET.Tags;

namespace ASN1Decoder.NET
{
    /// <summary>
    /// ASN1 Decoder
    /// </summary>
    public static class Decoder
    {
        #region privateMethods
        private static void ConvertTagsContentsToReadableStringsRecurse(ITag tag)
        {
            foreach (SmartTag child in tag.Children)
            {
                if (child.Children.Count > 0)
                    ConvertTagsContentsToReadableStringsRecurse(child);
                else
                    child.ConvertContentToReadableContent();
            }
        }

        private static string TagToStringRecurse(SmartTag tag, int lvl, string structureSpacer, int maxContentLineLength, ref string lastOid)
        {
            var sb = new StringBuilder();
            if (tag.Children.Count > 0)
            {
                sb.Append($"{MultiplyString(structureSpacer, lvl)}{tag.TagName}{Environment.NewLine}");
                foreach (SmartTag child in tag.Children)
                {
                    sb.Append(TagToStringRecurse(child, lvl + 1, structureSpacer, maxContentLineLength, ref lastOid));
                }
            }
            else
            {
                if (tag.TagNumber == 6)
                {
                    lastOid = tag.ReadableContent;
                }
                #region OID_SpecificProcessing
                if (tag.TagNumber == 3 && lastOid == "2.5.29.15, keyUsage, X.509 extension")
                {
                    ConvertKeyUsageFromBitStringToReadableString(tag, sb.ToString());
                }
                #endregion
                if (maxContentLineLength > 0 && tag.ReadableContent.Length > maxContentLineLength)
                {
                    string[] firstSplit = FormatContentByMaxCharactersPerLine(tag, lvl, structureSpacer, maxContentLineLength);

                    tag.ReadableContent = Environment.NewLine;
                    tag.ReadableContent += string.Join(Environment.NewLine, firstSplit);

                    tag.ReadableContent = tag.ReadableContent.TrimEnd();

                    sb.Append($"{MultiplyString(structureSpacer, lvl)}{tag.TagName} {tag.ReadableContent}{Environment.NewLine}");
                }
                else
                    sb.Append($"{MultiplyString(structureSpacer, lvl)}{tag.TagName} {tag.ReadableContent}{Environment.NewLine}");
            }

            return sb.ToString();
        }

        private static string[] FormatContentByMaxCharactersPerLine(SmartTag _tag, int lvl, string structureSpacer, int maxContentLineLength)
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

        private static string MultiplyString(string source, int multiplier)
        {
            if (multiplier > 3)
                return new StringBuilder().Insert(0, source, multiplier).ToString();

            string tmp = "";
            for (int i = 0; i < multiplier; i++)
            {
                tmp += source;
            }
            return tmp;
        }

        private static void ConvertKeyUsageFromBitStringToReadableString(SmartTag tag, string tmp)
        {
            var hex = BitConverter.ToString(tag.Content);
            var binSb = new StringBuilder();
            foreach (var item in hex.Split('-'))
            {
                binSb.Append(Convert.ToString(Convert.ToInt64(item, 16), 2));
            }
            //TODO: maybe use Span?
            string bin = binSb.ToString();
            bin = bin.PadRight(8, '0');
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator
            bin = bin.Substring(bin.Length - 8);
#pragma warning restore IDE0057 // Use range operator
#pragma warning restore IDE0079 // Remove unnecessary suppression

            if (tmp == "00000000")
            {
                tag.ReadableContent = "decipherOnly";
                return;
            }

            var kuSb = new StringBuilder();
            for (int i = 0; i < bin.Length; i++)
            {
                string _tmp = bin.Substring(i, 1);
                if (Enum.IsDefined(typeof(KU), i) && _tmp == "1")
                {
                    kuSb.Append($"{(KU)i}, ");
                }
            }
            tag.ReadableContent = kuSb.ToString();
        }

        #endregion

        #region publicAPI
        public static byte[] Encode(ITag data)
        {
            return ActualEncoder.Encode(data).ToArray();
        }

        /// <summary>
        /// Decode the DER encoded data sequence
        /// </summary>
        /// <param name="data">DER encoded data represented in bytes</param>
        /// <returns><see cref="SmartTag"/> containing the decoded sequence</returns>
        public static IReadOnlyTag Decode(byte[] data)
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
        public static string TagToString(IReadOnlyTag tag, string structureSpacer, int maxContentLineLength)
        {
            ConvertTagsContentsToReadableStringsRecurse(tag);
            string lastOid = null;
            if (tag is not SmartTag s)
            {
                s = (SmartTag)Decode(Encode(tag));
            }
            return TagToStringRecurse(s, 0, structureSpacer, maxContentLineLength, ref lastOid);
        }

        /// <summary>
        /// Destructurize the tag into list of tags
        /// </summary>
        /// <param name="tag">Top level tag</param>
        /// <returns>List of tag sorted by tag level</returns>
        public static List<ITag> Desctructurize(ITag tag)
        {
            List<ITag> list = new List<ITag>();
            if (tag.Children.Any())
            {
                list.Add(tag);
            }
            else
            {
                foreach (var item in tag.Children)
                {
                    list.AddRange(Desctructurize(item));
                }
            }
            return list;
        }
        #endregion
    }
}
