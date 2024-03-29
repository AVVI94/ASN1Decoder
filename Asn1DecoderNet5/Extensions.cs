﻿using System;
using System.Collections.Generic;
using System.Linq;
using Asn1DecoderNet5.Interfaces;
using Asn1DecoderNet5.Tags;
using Asn1DecoderNet5.Tags.SAN;

namespace Asn1DecoderNet5
{
    public static class Extensions
    {
        static readonly byte[] _version0Sequence = new byte[] { 0x00 };
        static readonly byte[] _version2Sequence = new byte[] { 0x02 };
        static readonly byte[] _booleanTrueSequence = new byte[] { 0xFF };
        static readonly byte[] _keyUsageOidSequence = OID.GetOrCreate(OID.KEY_USAGE).ByteValue;
        static readonly byte[] _sanOidSequence = OID.GetOrCreate(OID.SUBJECT_ALT_NAME).ByteValue;
        static readonly byte[] _icaUserIdSequence = OID.GetOrCreate(OID.ICA_USER_ID).ByteValue;
        static readonly byte[] _icaIkMpsvSequence = OID.GetOrCreate(OID.ICA_IK_MPSV).ByteValue;
        static readonly byte[] _extensionRequestSequence = OID.GetOrCreate(OID.EXTENSION_REQUEST).ByteValue;
        static readonly byte[] _extKeyUsageSequence = OID.GetOrCreate(OID.EXT_KEY_USAGE).ByteValue;

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

        //X.509 v3 https://www.rfc-editor.org/rfc/rfc5280
        //PKCS10 v1.7 https://datatracker.ietf.org/doc/html/rfc2986

        /// <summary>
        /// Attempts to find and parse key usage from the structure
        /// </summary>
        /// <param name="tag">Represents the element in which its children will be searched and parsed</param>
        /// <param name="keyUsage">The value</param>
        /// <returns><see langword="true" /> if KeyUsage was found, otherwise <see langword="false" /></returns>
        public static bool TryGetKeyUsage(this ITag tag, out KeyUsage keyUsage)
        {
            keyUsage = default;
            try
            {
                //if the tag is cert, the KeyUsage location is somewhere in cert's extensions
                if (IsCertificate(tag))
                    tag = tag.Childs[0].Childs[tag.Childs[0].Childs.Count - 1].Childs[0];
                return FindKeyUsage(tag, ref keyUsage);
            }
            catch
            {
                return false;
            }

            static bool FindKeyUsage(ITag tag, ref KeyUsage keyUsage)
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
                    if (FindKeyUsage(t, ref keyUsage))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Attempts to find the value for requested OID. Certificate can contain more than one value for requested OID, the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The Issuer and Subject elements are parsed based on this structure:
        /// The OID is matched based on the X.509 v3, meaning the OID is expected to have a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// and the value is taken from the second element of the SEQUENCE element.<br/>
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
        public static bool TryGetCertificateSubjectItem(this ITag topLevelTag, SubjectItemKind subjectItem, bool forIssuer, out List<string> items)
        {
            return TryGetCertificateSubjectItem(topLevelTag, subjectItem.ToOidString(), forIssuer, out items);
        }

        /// <summary>
        /// Attempts to find the value for requested OID. Certificate can contain more than one value for requested OID, the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The Issuer and Subject elements are parsed based on this structure:
        /// The OID is matched based on the X.509 v3, meaning the OID is expected to have a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// and the value is taken from the second element of the SEQUENCE element.<br/>
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
        public static bool TryGetCertificateSubjectItem(this ITag topLevelTag, string oid, bool forIssuer, out List<string> items)
        {
            items = new List<string>();
            if (IsOidNullOrEmpty(oid) || !IsCertificate(topLevelTag))
                return false;

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
        /// Attempts to find the value for the requested OID. A certificate can contain more than one value for the requested OID, and the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The OID is matched based on the PKCS10 CSR v1.7 specs, meaning the OID is expected to have a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// and the value is taken from the second element of the SEQUENCE element.<br/>
        /// Example of a correct SET element:
        /// <code>
        /// SET
        ///  | SEQUENCE
        ///  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
        ///  |  | UTF8String common name value
        /// </code>
        /// </remarks>
        /// <param name="topLevelTag">Tag object returned by <see cref="Decoder.Decode(byte[])"/></param>
        /// <param name="subjectItem">Requested item</param>
        /// <param name="items">The value</param>
        /// <returns><see langword="true" /> if any value was found, otherwise <see langword="false" /></returns>
        public static bool TryGetCertificateRequestSubjectItem(this ITag topLevelTag, SubjectItemKind subjectItem, out List<string> items)
        {
            return TryGetCertificateRequestSubjectItem(topLevelTag, subjectItem.ToOidString(), out items);
        }

        /// <summary>
        /// Attempts to find the value for the requested OID. A certificate can contain more than one value for the requested OID, and the values are ordered in the 'items' list as they were found in the structure.
        /// </summary>
        /// <remarks>
        /// The OID is matched based on the PKCS10 CSR v1.7 specs, meaning the OID is expected to have a parent element of type SEQUENCE, which has a parent element of type SET, 
        /// and the value is taken from the second element of the SEQUENCE element.<br/>
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
        /// <param name="items">The value</param>
        /// <returns><see langword="true" /> if any value was found, otherwise <see langword="false" /></returns>
        public static bool TryGetCertificateRequestSubjectItem(this ITag topLevelTag, string oid, out List<string> items)
        {
            items = new List<string>();
            try
            {
                if (IsOidNullOrEmpty(oid) || !IsCertificateRequest(topLevelTag))
                    return false;
                topLevelTag = topLevelTag.Childs[0].Childs[1];
                return TryGetSubjectItem(topLevelTag, Encoding.OidEncoding.GetBytes(oid), out items);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method attempts to locate the value for the requested OID. It's important to note that a certificate may contain multiple values for the requested OID, and these values are ordered in the 'items' list as they appear in the structure.
        /// </summary>
        /// <remarks>
        /// The OID is matched based on X.509 v3 standards, which means that the OID is expected to have a parent element of type SEQUENCE, which, in turn, should have a parent element of type SET.
        /// The value is then extracted from the second element of the SEQUENCE element.<br/>
        /// Example of a correct SET element:
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
        /// This method attempts to locate the value for the requested OID. It's important to note that a certificate may contain multiple values for the requested OID, and these values are ordered in the 'items' list as they appear in the structure.
        /// </summary>
        /// <remarks>
        /// The OID is matched based on X.509 v3 standards, which means that the OID is expected to have a parent element of type SEQUENCE, which, in turn, should have a parent element of type SET.
        /// The value is then extracted from the second element of the SEQUENCE element.<br/>
        /// Example of a correct SET element:
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
                    bool isSet = IsSET(t);
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
                => IsSET(t)
                   && t.Childs.Count == 1
                   && t.Childs[0].TagNumber == (int)Tags.Tags.SEQUENCE
                   && t.Childs[0].Childs[0].Content.SequenceEqual(oid);
        }

        /// <summary>
        /// Tries to retrieve the certificate's serial number. Returns <see langword="false" /> if the children (Childs property) are structured incorrectly or the appropriate element is missing.
        /// </summary>
        /// <param name="topLevelTag">Tag returned by <see cref="Decoder.Decode(byte[])"/> method</param>
        /// <param name="serialNumber">The serial number</param>
        /// <returns><see langword="true" /> if the element is found, otherwise <see langword="false" /></returns>
        public static bool TryCertificateGetSerialNumber(this ITag topLevelTag, out string serialNumber)
        {
            serialNumber = null;
            try
            {

                if (!IsCertificate(topLevelTag))
                    return false;
                serialNumber = GetValue(topLevelTag.Childs[0].Childs[1]);
                return serialNumber is not null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to retrieve the subject alternative name (SAN) and parse the known (and supported) tags from the certificate or certificate request.
        /// </summary>
        /// <remarks>
        /// To obtain the actual parsed data, you must cast the object from the result list according to its type.<br/><br/>
        /// <list type="table">
        /// <listheader>
        /// <term><see cref="SanItemKind"/></term>
        /// <description><see cref="Type"/> mapping</description>
        /// </listheader>
        /// <item>
        /// <term><see cref="SanItemKind.OtherName"/></term>
        /// <description><see cref="OtherName"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.Rfc822Name"/></term>
        /// <description><see cref="Rfc822Name"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.DNSName"/></term>
        /// <description><see cref="DnsName"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.X400Address"/></term>
        /// <description><see cref="X400Address"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.DirectoryName"/></term>
        /// <description><see cref="DirectoryName"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.EdiPartyName"/></term>
        /// <description><see cref="EdiPartyName"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.UniformResourceIdentifier"/></term>
        /// <description><see cref="UniformResourceIdentifier"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.IPAddress"/></term>
        /// <description><see cref="IPAddress"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="SanItemKind.RegisteredID"/></term>
        /// <description><see cref="RegisteredID"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="topLevelTag">Top level source tag</param>
        /// <param name="san">Result</param>
        /// <returns><see langword="true" /> if SAN was found, otherwise <see langword="false" /></returns>
        public static bool TryGetSubjectAlternativeName(this ITag topLevelTag, out List<ISanItem> san)
        {
            san = new();
            try
            {
                if (IsCertificate(topLevelTag))
                {
                    if (!HasCertExtensions(topLevelTag))
                        return false;
                    foreach (var item in GetCretificateExtensions(topLevelTag))
                    {
                        if (!item.Childs[0].Content.SequenceEqual(_sanOidSequence))
                            continue;
                        ParseSan(san, item);
                        break;
                    }
                }
                else if (IsCertificateRequest(topLevelTag))
                {
                    if (!HasRequestedExtensions(topLevelTag))
                        return false;
                    foreach (var item in GetRequestedExtensions(topLevelTag))
                    {
                        if (!item.Childs[0].Content.SequenceEqual(_sanOidSequence))
                            continue;
                        ParseSan(san, item);
                        break;
                    }
                }
                return true;

            }
            catch
            {
                return false;
            }

            static void ParseSan(List<ISanItem> san, ITag item)
            {
                foreach (var sanItem in item.Childs[1].Childs[0].Childs)
                {
                    switch (sanItem.TagName)
                    {
                        case "[0]":
                            if (sanItem.Childs[0].Content.SequenceEqual(_icaUserIdSequence))
                                san.Add(new IcaUserId(sanItem.Childs[1]));
                            else if (sanItem.Childs[0].Content.SequenceEqual(_icaIkMpsvSequence))
                                san.Add(new IcaIkMpsv(sanItem.Childs[1]));
                            else
                                san.Add(new OtherName(OID.GetOrCreate(sanItem.Childs[0].Content), sanItem.Childs[1].Childs[0]));
                            break;
                        case "[1]":
                            sanItem.ConvertContentToReadableContent();
                            san.Add(new Rfc822Name(sanItem.ReadableContent));
                            break;
                        case "[2]":
                            sanItem.ConvertContentToReadableContent();
                            san.Add(new DnsName(sanItem.ReadableContent));
                            break;
                        case "[3]":
                            sanItem.Childs[0].ConvertContentToReadableContent();
                            san.Add(new X400Address(sanItem.Childs[0]));
                            break;
                        case "[4]":
                            san.Add(new DirectoryName(sanItem.Childs[0]));
                            break;
                        case "[5]":
                            san.Add(new EdiPartyName(sanItem.Childs[0]));
                            break;
                        case "[6]":
                            sanItem.ConvertContentToReadableContent();
                            san.Add(new UniformResourceIdentifier(sanItem.ReadableContent));
                            break;
                        case "[7]":
                            sanItem.ConvertContentToReadableContent();
                            san.Add(new IPAddress(sanItem.ReadableContent));
                            break;
                        case "[8]":
                            sanItem.Childs[0].ConvertContentToReadableContent();
                            san.Add(new RegisteredID(OID.GetOrCreate(sanItem.Childs[0].Content)));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to retrieve the Extended Key Usage (EKU) from the specified certificate tag. If the EKU is found,
        /// the method sets the <paramref name="eku"/> parameter with the relevant information and returns <see langword="true"/>.
        /// If the EKU is not found, or if the certificate tag is invalid or lacks extensions, it returns <see langword="false"/>.
        /// </summary>
        /// <param name="topLevelCertificateTag">The certificate tag to extract Extended Key Usage from.</param>
        /// <param name="eku">An <see cref="ExtendedKeyUsageTag"/> object containing the EKU information if found.</param>
        /// <returns>
        /// <see langword="true"/> if any Extended Key Usage value was found and extracted successfully; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetExtendedKeyUsage(this ITag topLevelCertificateTag, out ExtendedKeyUsageTag eku)
        {
            eku = new ExtendedKeyUsageTag();
            try
            {
                if (!IsCertificate(topLevelCertificateTag) || !HasCertExtensions(topLevelCertificateTag))
                    return false;
                bool ekuFound = false;
                foreach (var ext in GetCretificateExtensions(topLevelCertificateTag))
                {
                    if (!ext.Childs[0].Content.SequenceEqual(_extKeyUsageSequence))
                        continue;
                    foreach (var e in ext.Childs[1].Childs[0].Childs)
                    {
                        switch (OID.GetOrCreate(e.Content).Value)
                        {
                            case OID.EKU_EMAIL_PROTECTION: eku.EmailProtection = true; ekuFound = true; break;
                            case OID.EKU_CLIENT_AUTH: eku.ClientAuth = true; ekuFound = true; break;
                            case OID.EKU_SERVER_AUTH: eku.ServerAuth = true; ekuFound = true; break;
                            case OID.EKU_TIME_STAMPING: eku.TimeStamping = true; ekuFound = true; break;
                            case OID.EKU_CODE_SIGNING: eku.CodeSigning = true; ekuFound = true; break;
                            case OID.EKU_OCSP_SIGNING: eku.OcspSigning = true; ekuFound = true; break;
                            default:
                                eku.OtherEKUs ??= new List<OID>();
                                ((List<OID>)eku.OtherEKUs).Add(OID.GetOrCreate(e.ReadableContent));
                                ekuFound = true;
                                break;
                        }
                    }
                }
                return ekuFound;
            }
            catch
            {
                return false;
            }
        }

#nullable enable
        public static bool TryGetICACertIntercon(this ITag topLevelCertificateTag, out ICACertIntercon intercon)
        {
            intercon = default;
            if (!IsCertificate(topLevelCertificateTag) || !HasCertExtensions(topLevelCertificateTag))
                return false;
            var ext = GetCretificateExtensions(topLevelCertificateTag);
            foreach (var item in ext)
            {
                if (OID.GetOrCreate(item.Childs[0].Content).Value != OID.ICA_CERT_INTERCONNECTION)
                    continue;
                var masterReqId = item.Childs[1].Childs[0].Childs[0];
                masterReqId.ConvertContentToReadableContent();
                var certCount = item.Childs[1].Childs[0].Childs[1];
                certCount.ConvertContentToReadableContent();
                intercon = new ICACertIntercon(item.Childs[1].Childs[0].Childs[2].Content.SequenceEqual(_booleanTrueSequence),
                                               masterReqId.ReadableContent,
                                               int.Parse(certCount.ReadableContent));
                return true;
            }
            return false;
        }
#nullable restore
        #region Helpers

        /// <summary>
        /// Checks whether the ASN.1 structure in this tag corresponds to X.509 v3.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns><see langword="true"/> if the children structure corresponds to X.509 v3, otherwise <see langword="false"/></returns>
        public static bool IsCertificate(this ITag tag)
        {
            try
            {
                return tag.Childs.Count == 3
                           && tag.Childs[0].Childs.Count != 0
                           && tag.Childs[0].Childs[0].TagClass == 2
                           && tag.Childs[0].Childs[0].Childs[0].TagNumber == (int)Tags.Tags.INTEGER
                           && tag.Childs[0].Childs[0].Childs[0].Content.SequenceEqual(_version2Sequence)
                           && tag.Childs[1].TagNumber == (int)Tags.Tags.SEQUENCE
                           && (tag.Childs[1].Childs.Count == 1 || tag.Childs[1].Childs.Count == 2)
                           && tag.Childs[1].Childs[0].TagNumber == (int)Tags.Tags.OBJECT_IDENTIFIER
                           && tag.Childs[2].Childs.Count == 0
                           && tag.Childs[2].TagNumber == (int)Tags.Tags.BIT_STRING;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Checks whether the ASN.1 structure in this tag corresponds to PKCS10 CSR v1.7 specification
        /// </summary>
        /// <param name="tag"></param>
        /// <returns><see langword="true"/> if the children structure corresponds to PKCS10 CSR v1.7 specification, otherwise <see langword="false"/></returns>
        public static bool IsCertificateRequest(this ITag tag)
        {
            try
            {
                return tag.Childs.Count == 3
                   && tag.Childs[0].Childs.Count != 0
                   && tag.Childs[0].Childs[0].TagNumber == (int)Tags.Tags.INTEGER
                   && tag.Childs[0].Childs[0].Content.SequenceEqual(_version0Sequence)
                   && tag.Childs[1].TagNumber == (int)Tags.Tags.SEQUENCE
                   && (tag.Childs[1].Childs.Count == 1 || tag.Childs[1].Childs.Count == 2)
                   && tag.Childs[1].Childs[0].TagNumber == (int)Tags.Tags.OBJECT_IDENTIFIER
                   && tag.Childs[2].Childs.Count == 0
                   && tag.Childs[2].TagNumber == (int)Tags.Tags.BIT_STRING;
            }
            catch
            {
                return false;
            }
        }

        public static string ToPrettyString(this ITag tag, string elementIndentationRepresentation = " | ", int maxLineLength = 128)
        {
            return Decoder.TagToString(tag, elementIndentationRepresentation, maxLineLength);
        }

        static bool HasCertExtensions(ITag topLevelCertificateTag)
        {
            return topLevelCertificateTag.Childs[0].Childs[topLevelCertificateTag.Childs[0].Childs.Count - 1].TagName == "[3]";
        }
        static bool HasRequestedExtensions(ITag topLevelCertRequestTag)
        {
            return topLevelCertRequestTag.Childs[0].Childs[topLevelCertRequestTag.Childs[0].Childs.Count - 1].TagName == "[0]"
                    && topLevelCertRequestTag.Childs[0].Childs[topLevelCertRequestTag.Childs[0].Childs.Count - 1]
                                  .Childs[0].Childs[0].Content.SequenceEqual(_extensionRequestSequence);
        }
        static bool IsSET(ITag t) => t.TagNumber == (int)Tags.Tags.SET;
        static string GetValue(ITag t)
        {
            if (t.Childs[0].Childs[1].ReadableContent is null)
                t.Childs[0].Childs[1].ConvertContentToReadableContent();
            return t.Childs[0].Childs[1].ReadableContent;
        }
        static bool IsOidNullOrEmpty(string oid)
        {
            return string.IsNullOrWhiteSpace(oid);
        }
        static bool IsOidNullOrEmpty(byte[] oid)
        {
            return oid is null or { Length: 0 };

        }
        static List<ITag> GetCretificateExtensions(ITag topLevelTag)
        {
            return topLevelTag.Childs[0].Childs[topLevelTag.Childs[0].Childs.Count - 1].Childs[0].Childs;
        }
        static List<ITag> GetRequestedExtensions(ITag topLevelTag)
        {
            return topLevelTag.Childs[0].Childs[topLevelTag.Childs[0].Childs.Count - 1].Childs[0].Childs[1].Childs[0].Childs;
        }
        internal static bool IsKeyUsageSequence(this ITag tag)
            => tag.Childs.Count == 3 && tag.Childs[0] is { TagNumber: (int)Tags.Tags.OBJECT_IDENTIFIER } oidTag && oidTag.Content.SequenceEqual(_keyUsageOidSequence);
        internal static bool IsKeyUsageCritical(this ITag tag)
            => tag.IsKeyUsageSequence() && tag.Childs[1] is { TagNumber: (int)Tags.Tags.BOOLEAN } boolTag && boolTag.Content.SequenceEqual(_booleanTrueSequence);
        internal static ITag GetKeyUsageBitStringTag(this ITag tag)
            => tag.Childs[2].Childs[0];

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

    /// <summary>
    /// Represents an I.CA Certificate Interconnection structure.
    /// </summary>
    public readonly record struct ICACertIntercon
    {
        /// <summary>
        /// Creates a new instance of the I.CA Certificate Interconnection
        /// </summary>
        /// <param name="isMaster">Specifies if the certificate this structure was parsed from is a master certificate</param>
        /// <param name="masterRequestId">The master's certificate request ID</param>
        /// <param name="interconnectedCertificatesCount">The number of certificates that are interconnected</param>
        public ICACertIntercon(bool isMaster, string masterRequestId, int interconnectedCertificatesCount)
        {
            IsMaster = isMaster;
            MasterRequestId = masterRequestId;
            InterconnectedCertificatesCount = interconnectedCertificatesCount;
        }

        /// <summary>
        /// Indicating whether the certificate is a master certificate
        /// </summary>
        public readonly bool IsMaster { get; }

        /// <summary>
        /// The master's certificate request ID
        /// </summary>
        public readonly string MasterRequestId { get; }

        /// <summary>
        /// The number of certificates that are interconnected
        /// </summary>
        public readonly int InterconnectedCertificatesCount { get; }
    }
}
