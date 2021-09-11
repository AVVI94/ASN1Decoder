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
