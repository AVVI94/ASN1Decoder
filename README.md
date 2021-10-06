# ASN1Decoder
This library aims to make decoding DER encoded data into ASN1 structure human readable easier. So, you pass in the data and the library returns you an object containing the whole ASN1 sequence and provides you a function that can convert this object into a pretty string.

# How to use
1. install the Nuget package (www.nuget.org/packages/Asn1DecoderNet5) or clone the repo, build the project and reference the library
```
Install-Package Asn1DecoderNet5
```
3. add Asn1DecoderNet5 to your usings statements
```CSharp
using Asn1DecoderNet5;
```
4. read a DER file bytes
```CSharp
byte[] derBytes = File.ReadAllBytes("derFile.der");
```
5. pass the bytes into Decoder.Decode method and save its output into a variable
```CSharp
ITag tag = Decoder.Decode(derBytes);
```
6. pass the variable into Decoder.TagToString method, specify the string to structurize the string (I reccomend " | ") and maximum length of one line (excluding the formatter string)
```CSharp
string result = Decoder.TagToString(tag, " | ", 128);
```
7. enjoy the string (or an exception)
```Text
SEQUENCE
| OBJECT_IDENTIFIER 1.2.840.113549.1.7.2, signedData, PKCS #7
| [0]
|  | SEQUENCE
|  |  | INTEGER 1
|  |  | SET
...
```

# Full code example
www.github.com/AVVI94/ASN1Decoder/blob/main/Asn1Decoder.ConsoleDecoder/Program.cs



# Full output example
https://pastebin.com/0LVC0wKE
