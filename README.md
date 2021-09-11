# ASN1Decoder
This library aims to make decoding DER encoded data into ASN1 structure human readable easier. So, you pass in the data and the library returns you an object containing the whole ASN1 sequence and provides you a function that can convert this object into a pretty string.

# How to use
1. install the Nuget package (www.nuget.org/packages/Asn1DecoderNet5) or clone the repo, build the project and reference the library
2. add using Asn1DecoderNet5; to your usings
3. read a DER file bytes
4. pass the bytes into Decoder.Decode method and save its output into a variable
5. pass the variable into Decoder.TagToString method, specify the string to structurize the string (I reccomend " | ") and maximum length of one line
6. enjoy the string (or an exception)

# How to use in code
See www.github.com/AVVI94/ASN1Decoder/blob/main/Asn1Decoder.ConsoleDecoder/Program.cs



# Output example
SEQUENCE
| OBJECT_IDENTIFIER 1.2.840.113549.1.7.2, signedData, PKCS #7
| [0]
|  | SEQUENCE
|  |  | INTEGER 1
|  |  | SET
|  |  |  | SEQUENCE
|  |  |  |  | OBJECT_IDENTIFIER 2.16.840.1.101.3.4.2.1, sha-256, NIST Algorithm
|  |  | SEQUENCE
|  |  |  | OBJECT_IDENTIFIER 1.2.840.113549.1.7.1, data, PKCS #7
|  |  |  | [0]
|  |  |  |  | OCTET_STRING Test signature
|  |  | [0]
|  |  |  | SEQUENCE
|  |  |  |  | SEQUENCE
|  |  |  |  |  | [0]
|  |  |  |  |  |  | INTEGER 2
|  |  |  |  |  | INTEGER 97770
|  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  | OBJECT_IDENTIFIER 1.2.840.113549.1.1.11, sha256WithRSAEncryption, PKCS #1
|  |  |  |  |  |  | NULL
|  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.6, countryName, X.520 DN component
|  |  |  |  |  |  |  |  | PrintableString CZ
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String I.CA Test Public CA/RSA 11/2015
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.10, organizationName, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String První certifikační autorita, a.s.
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.5, serialNumber, X.520 DN component
|  |  |  |  |  |  |  |  | PrintableString NTRCZ-26439395
|  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  | UTCTime 2021/05/26 10:45:35 UTC
|  |  |  |  |  |  | UTCTime 2022/05/26 10:45:35 UTC
|  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String Jan Král
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.6, countryName, X.520 DN component
|  |  |  |  |  |  |  |  | PrintableString CZ
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.10, organizationName, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String První certifikační autorita, a.s.
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.42, givenName, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String Jan
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.4, surname, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String Král
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.5, serialNumber, X.520 DN component
|  |  |  |  |  |  |  |  | PrintableString ICA - 65248
|  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.2.840.113549.1.1.1, rsaEncryption, PKCS #1
|  |  |  |  |  |  |  | NULL
|  |  |  |  |  |  | BIT_STRING
|  |  |  |  |  |  |  | 001100001000001000000001000010100000001010000010000000010000000
|  |  |  |  |  |  |  | 000101110101011101100100001111100001000010101011010011111101001
|  |  |  |  |  |  |  | 111100100011110010111111110110010001011111101000101011101110000
|  |  |  |  |  |  |  | 100001000000011111010110110111111000001001110010100111111111101
|  |  |  |  |  |  |  | 010000001110000000010000111011000010000000001011101011000000100
|  |  |  |  |  |  |  | 110000001101001011101110011110101010000010010100110110001100101
|  |  |  |  |  |  |  | 001111101011101001111101010101101001101011010110100010110011110
|  |  |  |  |  |  |  | 010001010011100100100001001110101010101110101100111000001111001
|  |  |  |  |  |  |  | 011001001101100110000100011001110001111010100011010110100110000
|  |  |  |  |  |  |  | 101110100001111110000011001010110010100100101010000100101111001
|  |  |  |  |  |  |  | 001111100101001111100111001010001011010011110001010001110100111
|  |  |  |  |  | [3]
|  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.17, subjectAltName, X.509 extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  | [1] kral@ica.cz
|  |  |  |  |  |  |  |  |  |  | [0]
|  |  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.4.1.23624.4.6, I.CA_User_ID, I.CA User ID
|  |  |  |  |  |  |  |  |  |  |  | [0]
|  |  |  |  |  |  |  |  |  |  |  |  | UTF8String 65248
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.15, keyUsage, X.509 extension
|  |  |  |  |  |  |  |  | BOOLEAN True
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | BIT_STRING digitalSignature, nonRepudiation, keyEncipherment,
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.32, certificatePolicies, X.509 extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.4.1.23624.10.3.70.1.1
|  |  |  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.5.5.7.2.1, cps, PKIX policy qualifier
|  |  |  |  |  |  |  |  |  |  |  |  |  | IA5String http://www.ica.cz
|  |  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 0.4.0.2042.1.1
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.31, cRLDistributionPoints, X.509 extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  |  | [0]
|  |  |  |  |  |  |  |  |  |  |  |  | [0]
|  |  |  |  |  |  |  |  |  |  |  |  |  | [6] http://tests.ica.cz/tpca15_rsa.crl
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.5.5.7.1.1, authorityInfoAccess, PKIX private extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.5.5.7.48.2, caIssuers, PKIX subject/authority info access descriptor
|  |  |  |  |  |  |  |  |  |  |  | [6] http://tests.ica.cz/tpca15_rsa.cer
|  |  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.5.5.7.48.1, ocsp, PKIX
|  |  |  |  |  |  |  |  |  |  |  | [6] http://tocsp.ica.cz/tpca15_rsa
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.19, basicConstraints, X.509 extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.35, authorityKeyIdentifier, X.509 extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  | [0] 432AFDFF9F8B963F069118E7CBB553425CB0F5C8
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.14, subjectKeyIdentifier, X.509 extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | OCTET_STRING 631A854E09CEF54921D6C2DC45F2DA1B27F90768
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.29.37, extKeyUsage, X.509 extension
|  |  |  |  |  |  |  |  | OCTET_STRING
|  |  |  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.5.5.7.3.2, clientAuth, PKIX key purpose
|  |  |  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 1.3.6.1.5.5.7.3.4, emailProtection, PKIX key purpose
|  |  |  |  | SEQUENCE
|  |  |  |  |  | OBJECT_IDENTIFIER 1.2.840.113549.1.1.11, sha256WithRSAEncryption, PKCS #1
|  |  |  |  |  | NULL
|  |  |  |  | BIT_STRING
|  |  |  |  |  | 000000001000110100100100111000101010001110010110110010110110001
|  |  |  |  |  | 011100001011100001001110010101001000110110000001000011011010000
|  |  |  |  |  | 011010000100001010111101100110111110100111110000101100001111100
|  |  |  |  |  | 001111001000010001110111111011110010101001001100011011110010000
|  |  |  |  |  | 100110001001010111110001101101001111001111110001000100110111111
|  |  |  |  |  | 000101010100010111101010001001100111111001011110001000001110001
|  |  |  |  |  | 101000010000000001101101001000011100110001010000011001011010101
|  |  |  |  |  | 111101111101110111100011100001101111100100111001010010010001010
|  |  |  |  |  | 010001110100000011100011100001111011100101111101011001000000101
|  |  |  |  |  | 100110110000010000110010011010010010111001000100110001011011001
|  |  |  |  |  | 100011000110101010010001111001101110110001010110000100100111111
|  |  |  |  |  | 010101111111010010010001001011110000101100001100100011011110101
|  |  |  |  |  | 010111010001000011010100010010000100101010111111100110000001010
|  |  |  |  |  | 110010011111011100011010010000001010000110001110100110111011111
|  |  |  |  |  | 101011100110101001011000111101101111001000001100010010100100001
|  |  |  |  |  | 001001000011011110010001000001100000100000001110100011101100111
|  |  |  |  |  | 110000001111101010000000011010010011011110110111011001111101100
|  |  |  |  |  | 011010111011011110000000011110101011111000111101110010100100110
|  |  |  |  |  | 111011110001110100110011100101000101101111001110110011001101110
|  |  |  |  |  | 101001110100010101010011000101011000100111010000001111101011011
|  |  |  |  |  | 011100110001100111101110011000011000101001001110100111010100000
|  |  | SET
|  |  |  | SEQUENCE
|  |  |  |  | INTEGER 1
|  |  |  |  | SEQUENCE
|  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.6, countryName, X.520 DN component
|  |  |  |  |  |  |  |  | PrintableString CZ
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.3, commonName, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String I.CA Test Public CA/RSA 11/2015
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.10, organizationName, X.520 DN component
|  |  |  |  |  |  |  |  | UTF8String První certifikační autorita, a.s.
|  |  |  |  |  |  | SET
|  |  |  |  |  |  |  | SEQUENCE
|  |  |  |  |  |  |  |  | OBJECT_IDENTIFIER 2.5.4.5, serialNumber, X.520 DN component
|  |  |  |  |  |  |  |  | PrintableString NTRCZ-26439395
|  |  |  |  |  | INTEGER 97770
|  |  |  |  | SEQUENCE
|  |  |  |  |  | OBJECT_IDENTIFIER 2.16.840.1.101.3.4.2.1, sha-256, NIST Algorithm
|  |  |  |  | SEQUENCE
|  |  |  |  |  | OBJECT_IDENTIFIER 1.2.840.113549.1.1.1, rsaEncryption, PKCS #1
|  |  |  |  | OCTET_STRING
|  |  |  |  |  | 2358C0EF1039020D8724B8783A6D1EA274F870D8AD94D7DEAFA43FC5FEBF675
|  |  |  |  |  | 8559CB6C752AE5B00E4F8C61F047B9A9C6294D74E3AA7F96D37D0B8D3AB35EC
|  |  |  |  |  | 2673586B8D1A91121B8DC142F0832DEC6B2551F3F16FEB518BE8CB39D8EDED8
