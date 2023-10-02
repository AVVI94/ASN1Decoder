using System;
using System.Collections.Generic;
using System.Linq;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;

namespace Asn1DecoderNet5
{
    public static class Extensions
    {
        public static string ToOidString(this SubjectItemKind subjectItem)
        {
            return subjectItem switch
            {
                SubjectItemKind.CommonName => OID.COMMON_NAME,
                SubjectItemKind.GivenName => OID.GIVEN_NAME,
                SubjectItemKind.Surname => OID.SURNAME,
                SubjectItemKind.CountryName => OID.COUNTRY_NAME,
                SubjectItemKind.OrganizationName => OID.ORGANIZATION_NAME,
                SubjectItemKind.OrganizationUnit => OID.ORGANIZATION_UNIT,
                SubjectItemKind.StateOrProvinceName => OID.STATE_OR_PROVINCE_NAME,
                SubjectItemKind.Locality => OID.LOCALITY,
                SubjectItemKind.SerialNumber => OID.SERIAL_NUMBER,
                SubjectItemKind.Title => OID.TITLE,
                _ => null//throw new ArgumentException($"Parameter '{nameof(subjectItem)}' has invalid value!", nameof(subjectItem)),
            };
        }
        #region KeyUsage
        static readonly byte[] _booleanTrueSequence = new byte[] { 0xFF };
        static readonly byte[] _keyUsageOidSequence = new byte[] { 0x55, 0x1D, 0x0f };

        /// <summary>
        /// Attempts to find and parse key usage from the structure
        /// </summary>
        /// <param name="tag">Element whose children will be searched and parsed</param>
        /// <param name="keyUsage">The value</param>
        /// <returns><see langword="true" /> if KeyUsage was found, otherwise <see langword="false" /></returns>
        public static bool TryGetKeyUsage(this ITag tag, out KeyUsage keyUsage)
        {
            keyUsage = default;
            try
            {
                if (tag is KeyUsageTag { KeyUsage.KeyUsages: not null } kut)
                {
                    keyUsage = kut.KeyUsage;
                    return true;
                }
                else if (tag.IsKeyUsageSequence())
                {
                    keyUsage = new KeyUsageTag(tag.GetKeyUsageBitStringTag(), tag.IsKeyUsageCritical()).KeyUsage;
                    return true;
                }

                foreach (var t in tag.Childs)
                {
                    if (t.TryGetKeyUsage(out keyUsage))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        internal static bool IsKeyUsageSequence(this ITag tag)
            => tag.Childs.Count == 3 && tag.Childs[0] is { TagNumber: (int)Tags.Tags.OBJECT_IDENTIFIER } oidTag && oidTag.Content.SequenceEqual(_keyUsageOidSequence);
        internal static bool IsKeyUsageCritical(this ITag tag)
            => tag.IsKeyUsageSequence() && tag.Childs[1] is { TagNumber: (int)Tags.Tags.BOOLEAN } boolTag && boolTag.Content.SequenceEqual(_booleanTrueSequence);
        internal static ITag GetKeyUsageBitStringTag(this ITag tag)
            => tag.Childs[2].Childs[0];
        #endregion

        /// <summary>
        /// Attempts to find the value for requested OID. Certificate can contain more than one value for requested OID, the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The Issuer and Subject elements are parsed based on this structure:
        /// <code>
        /// this.Childs[0]
        ///         .Childs[0]: Version
        ///         .Childs[1]: Serial Number
        ///         .Childs[2]: Signature Algorithm
        ///         .Childs[3]: Issuer
        ///         .Childs[4]: Validity
        ///             .Childs[0]:  - Not Before
        ///             .Childs[1]:  - Not After
        ///         .Childs[5]: Subject
        ///         .Childs[6]: Public Key Information
        ///             .Childs[0]:  - Public Key Algorithm
        ///             .Childs[1]:  - Public Key
        /// </code>
        /// <br/>
        /// <br/>
        /// The OID is matched based on the X.509 v3, meaning the OID is expected to has a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// the value is taken from the second element of the SEQUENCE element.<br/>
        /// Example of a correct SET element:
        /// <code>
        /// SET
        ///  | SEQUENCE
        ///  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
        ///  |  | UTF8String common name value
        /// </code>
        /// </remarks>
        /// <param name="topLevelTag">Tag object returned by <see cref="Decoder.Decode(byte[])"/></param>
        /// <param name="subjectItem">Subject item kind</param>
        /// <param name="forIssuer">Indicates, whether to find the value for requested OID in the Issuer element or in the Subject element</param>
        /// <param name="items">The value</param>
        /// <returns><see langword="true" /> if any value was found, otherwise <see langword="false" /></returns>
        public static bool TryGetSubjectItem(this ITag topLevelTag, SubjectItemKind subjectItem, bool forIssuer, out List<string> items)
        {
            return TryGetSubjectItem(topLevelTag, subjectItem.ToOidString(), forIssuer, out items);
        }

        /// <summary>
        /// Attempts to find the value for requested OID. Certificate can contain more than one value for requested OID, the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The Issuer and Subject elements are parsed based on this structure:
        /// <code>
        /// this.Childs[0]
        ///         .Childs[0]: Version
        ///         .Childs[1]: Serial Number
        ///         .Childs[2]: Signature Algorithm
        ///         .Childs[3]: Issuer
        ///         .Childs[4]: Validity
        ///             .Childs[0]:  - Not Before
        ///             .Childs[1]:  - Not After
        ///         .Childs[5]: Subject
        ///         .Childs[6]: Public Key Information
        ///             .Childs[0]:  - Public Key Algorithm
        ///             .Childs[1]:  - Public Key
        /// </code>
        /// <br/>
        /// <br/>
        /// The OID is matched based on the X.509 v3, meaning the OID is expected to has a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// the value is taken from the second element of the SEQUENCE element.<br/>
        /// Example of a correct SET element:
        /// <code>
        /// SET
        ///  | SEQUENCE
        ///  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
        ///  |  | UTF8String common name value
        /// </code>
        /// </remarks>
        /// <param name="topLevelTag">Tag object returned by <see cref="Decoder.Decode(byte[])"/></param>
        /// <param name="oid">Requested OID</param>
        /// <param name="forIssuer">Indicates, whether to find the value for requested OID in the Issuer element or in the Subject element</param>
        /// <param name="items">The value</param>
        /// <returns><see langword="true" /> if any value was found, otherwise <see langword="false" /></returns>
        public static bool TryGetSubjectItem(this ITag topLevelTag, string oid, bool forIssuer, out List<string> items)
        {
            items = new List<string>();
            if (IsOidNullOrEmpty(oid))
                return false;
            /*
             SEQUENCE               - tag
                | SEQUENCE          - tag.Childs[0]
                |  | [0]            - tag.Childs[0].Childs[0]
                |  |  | INTEGER 2   - tag.Childs[0].Childs[0].Childs[0]
                |  | INTEGER 36252270966868523741 tag.Childs[0].Childs[1]
             */
            /*
             this.Childs[0]
                    .Childs[0]: Version
                    .Childs[1]: Serial Number
                    .Childs[2]: Signature Algorithm
                    .Childs[3]: Issuer
                    .Childs[4]: Validity
                        .Childs[0]:  - Not Before
                        .Childs[1]:  - Not After
                    .Childs[5]: Subject
                    .Childs[6]: Public Key Information
                        .Childs[0]:  - Public Key Algorithm
                        .Childs[1]:  - Public Key
                    .Childs[7]: Signature
             */

            try
            {
                if (topLevelTag.Childs.Count == 0)
                    return false;   //("Tag cannot have zero childs", nameof(tag));
                if (topLevelTag.Childs[0].Childs.Count == 0)
                    return false;   //("First child tag cannot have zero childs", nameof(tag));
                if (topLevelTag.Childs[0].Childs[0].Childs[0].TagNumber != (int)Tags.Tags.INTEGER)
                    return false;   // ($"Certificate version tag was not found at position '{nameof(tag)}.Childs[0].Childs[0].Childs[0]'", nameof(tag));
                if (HasValidSn(topLevelTag))
                    return false;   //($"Certificate's serial number tag was not found at position '{nameof(tag)}.Childs[0].Childs[1]'", nameof(tag));
                if (topLevelTag.Childs[0].Childs[4].Childs[0].TagNumber != (int)Tags.Tags.UTCTime)
                    return false;   //($"Certificate's not after tag was not found at position '{nameof(tag)}.Childs[0].Childs[4].Childs[0]'", nameof(tag));
            }
            catch
            {
                return false;
            }

            if (forIssuer)
            {
                return TryGetSubjectItem(topLevelTag.Childs[0].Childs[3], Encoding.OidEncoding.GetBytes(oid), out items);
            }
            else
            {
                return TryGetSubjectItem(topLevelTag.Childs[0].Childs[5], Encoding.OidEncoding.GetBytes(oid), out items);
            }
        }

        /// <summary>
        /// Attempts to find the value for requested OID. Certificate can contain more than one value for requested OID, the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The OID is matched based on the X.509 v3, meaning the OID is expected to has a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// the value is taken from the second element of the SEQUENCE element.<br/>
        /// Example of correct SET element:
        /// <code>
        /// SET
        ///  | SEQUENCE
        ///  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
        ///  |  | UTF8String common name value
        /// </code>
        /// </remarks>
        /// <param name="tag">Parent tag</param>
        /// <param name="kind">Subject item kind</param>
        /// <param name="items">The value</param>
        /// <returns><see langword="true" /> if any value was found, otherwise <see langword="false" /></returns>
        public static bool TryGetSubjectItem(this ITag tag, SubjectItemKind kind, out List<string> items)
        {
            return TryGetSubjectItem(tag, Encoding.OidEncoding.GetBytes(kind.ToOidString()), out items);
        }

        /// <summary>
        /// Attempts to find the value for requested OID. Certificate can contain more than one value for requested OID, the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The OID is matched based on the X.509 v3, meaning the OID is expected to has a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// the value is taken from the second element of the SEQUENCE element.<br/>
        /// Example of correct SET element:
        /// <code>
        /// SET
        ///  | SEQUENCE
        ///  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
        ///  |  | UTF8String common name value
        /// </code>
        /// </remarks>
        /// <param name="tag">Parent tag</param>
        /// <param name="oid">Requested OID in byte array representation. <see cref="Asn1DecoderNet5.Encoding.OidEncoding"/> can be used to convert it from a string representation.</param>
        /// <param name="items">The value</param>
        /// <returns><see langword="true" /> if any value was found, otherwise <see langword="false" /></returns>
        public static bool TryGetSubjectItem(this ITag tag, byte[] oid, out List<string> items)
        {
            items = new List<string>();
            if (IsOidNullOrEmpty(oid))
                return false;
            /*
             SET
              | SEQUENCE
              |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
              |  | UTF8String common name value
             */
            if (tag.Childs.Count == 0)
                return false;
            try
            {
                if (IsValidSet(tag, oid))
                {
                    items.Add(GetValue(tag));
                }
                foreach (var t in tag.Childs)
                {
                    bool isSet = IsSet(t);
                    if (!isSet && t.Childs.Count > 0 && TryGetSubjectItem(t, oid, out var items2))
                    {
                        items.AddRange(items2);
                        continue;
                    }
                    if (IsValidSet(t, oid))
                    {
                        items.Add(GetValue(t));
                    }
                }
                return items.Count > 0;
            }
            catch
            {
                return false;
            }
            static bool IsValidSet(ITag t, byte[] oid)
                => IsSet(t)
                   && t.Childs.Count == 1
                   && t.Childs[0].TagNumber == (int)Tags.Tags.SEQUENCE
                   && t.Childs[0].Childs[0].Content.SequenceEqual(oid);
        }

        /// <summary>
        /// Tries to retrieve the certificate's serial number. Returns <see langword="false" /> if the children (Childs property) are structured incorrectly or the appropriate element is missing.
        /// </summary>
        /// <remarks>
        /// The serial number tag is expected to be at this position:<br/>
        /// <code>
        /// this.Childs[0].Childs[1]
        /// </code>
        /// </remarks>
        /// <param name="topLevelTag">Tag returned by <see cref="Decoder.Decode(byte[])"/> method</param>
        /// <param name="serialNumber">The serial number</param>
        /// <returns><see langword="true" /> if the element is found, otherwise <see langword="false" /></returns>
        public static bool TryGetSerialNumber(this ITag topLevelTag, out string serialNumber)
        {
            serialNumber = null;
            try
            {
                if (!HasValidSn(topLevelTag))
                    return false;
                serialNumber = GetValue(topLevelTag.Childs[0].Childs[1]);
                return serialNumber is not null;
            }
            catch
            {
                return false;
            }
        }

        #region Helpers
        static bool IsSet(ITag t) => t.TagNumber == (int)Tags.Tags.SET;
        static string GetValue(ITag t)
        {
            if (t.Childs[0].Childs[1].ReadableContent is null)
                t.Childs[0].Childs[1].ConvertContentToReadableContent();
            return t.Childs[0].Childs[1].ReadableContent;
        }
        static bool HasValidSn(ITag t) => t.Childs[0].Childs[1].TagNumber != (int)Tags.Tags.INTEGER;

        static bool IsOidNullOrEmpty(string oid)
        {
            return string.IsNullOrWhiteSpace(oid);
        }

        static bool IsOidNullOrEmpty(byte[] oid)
        {
            return oid is null or { Length: 0 };

        }
        #endregion
    }

    public enum SubjectItemKind
    {
        CommonName,
        GivenName,
        Surname,
        CountryName,
        OrganizationName,
        OrganizationUnit,
        StateOrProvinceName,
        Locality,
        SerialNumber,
        Title,
    }
}
