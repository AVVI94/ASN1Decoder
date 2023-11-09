# ASN1Decoder
This library aims to facilitate the parsing of ASN.1 data. It provides a data decoder and a set of methods for their parsing.

NuGet package: www.nuget.org/packages/Asn1DecoderNet5

## Decoding

The class `Asn1DecoderNet5.Decoder` is used for decoding DER data. Specifically, the static method `Decode` returns an `Asn1DecoderNet5.Interfaces.ITag` object.

```cs
ITag decodedDataStructure = Asn1DecoderNet5.Decoder.Decode(dataByteArray);
```

## Working with the decoded data

### Get a string representation

To obtain the string representation of the ASN.1 structure, you can utilize this extension method:

```cs
string result = decodedDataStructure.ToPrettyString(" | ", 128);
```

This method accepts two parameters. The first parameter is a string used to control the indentation of each element. The second parameter is the maximum length of a single line in the resulting string, **not** including the indentation.

The resulting string will appear as follows:

```Text
SEQUENCE
| OBJECT_IDENTIFIER 1.2.840.113549.1.7.2, signedData, PKCS #7
| [0]
|  | SEQUENCE
|  |  | INTEGER 1
|  |  | SET
...
```

### Check if the data is X.509 v3 certificate

To check if the data is actual X.509 certificate use this extension method:

```cs
bool isCert = decodedDataStructure.IsCertificate();
```

### Get items SubjectAlternativeName 

To get items from the Subject Alternative Name (SAN), you can refer to the unit tests provided in the [source code](https://github.com/AVVI94/ASN1Decoder/blob/main/Asn1DecoderNet5.UnitTests/DecoderTests.cs). The unit tests in the linked file will demonstrate how to use the library to extract and work with items from the SAN in ASN.1 data.

TL;DR:

```cs
[Fact]
public void Should_GetSan_GetsSanWithMailFromCert()
{
    var tag = Decoder.Decode(Sources.NormalCertificateWithEmailInSan);
    var ok = tag.TryGetSubjectAlternativeName(out var san);
    Assert.True(ok);
    Assert.NotEmpty(san);
    Tags.SAN.Rfc822Name email = null;
    foreach (var item in san)
    {
        if (item.ItemKind == Tags.SAN.SanItemKind.Rfc822Name)
        {
            email = (Tags.SAN.Rfc822Name)item;
            break;
        }
    }
    Assert.NotNull(email);
    Assert.Equal("kral@ica.cz", email.Content);
}
```
