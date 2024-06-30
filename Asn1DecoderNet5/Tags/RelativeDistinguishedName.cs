using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASN1Decoder.NET.Tags;
public class RelativeDistinguishedName
{
    public RelativeDistinguishedName(OID oid, string value)
    {
        ObjectIdentifier = oid;
        Value = value;
    }
    public RelativeDistinguishedName()
    {
    }

    public OID ObjectIdentifier { get; protected set; }
    public string Value { get; protected set; }
}
