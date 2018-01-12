using System.Collections.Generic;
using System.Text;

namespace Tiveria.Knx.IP.Utils
{
    /**
     * KNXnet/IP Host Protocol Address Information (HPAI).
     * 
     * The address information describing a communication channel. 
     * UDP is the default communication mode with mandatory support used in KNXnet/IP.
     */

    public enum HPAIEndpointType
    {
        IPV4_UDP = 0x01,
        IPV4_TCP = 0x02
    }
}
