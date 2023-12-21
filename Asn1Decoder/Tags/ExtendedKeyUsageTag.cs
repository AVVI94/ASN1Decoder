using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asn1Decoder.Tags;

#nullable enable
public struct ExtendedKeyUsageTag
{
    public ExtendedKeyUsageTag(bool clientAuth = false,
                               bool serverAuth = false,
                               bool codeSigning = false,
                               bool emailProtection = false,
                               bool timeStamping = false,
                               bool ocspSigning = false) : this()
    {
        ClientAuth = clientAuth;
        ServerAuth = serverAuth;
        CodeSigning = codeSigning;
        EmailProtection = emailProtection;
        TimeStamping = timeStamping;
        OcspSigning = ocspSigning;
    }

    public bool ClientAuth { get; internal set; }
    public bool ServerAuth { get; internal set; }
    public bool CodeSigning { get; internal set; }
    public bool EmailProtection { get; internal set; }
    public bool TimeStamping { get; internal set; }
    public bool OcspSigning { get; internal set; }
    public IReadOnlyList<OID>? OtherEKUs { get; internal set; }
}
