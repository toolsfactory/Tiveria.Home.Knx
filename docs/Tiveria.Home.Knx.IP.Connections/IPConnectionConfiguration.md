# IPConnectionConfiguration record

Configuration options for a kinds of KnxNetIP connections

```csharp
public record IPConnectionConfiguration
```

## Public Members

| name | description |
| --- | --- |
| [IPConnectionConfiguration](IPConnectionConfiguration/IPConnectionConfiguration.md)() | The default constructor. |
| [AcknowledgeTimeout](IPConnectionConfiguration/AcknowledgeTimeout.md) { get; set; } | Timeout after the achnowledge is deemed to not happen anymore |
| [NatAware](IPConnectionConfiguration/NatAware.md) { get; set; } | Switch NAT Awarenes of the cennection initiation |
| [ResyncSequenceNumbers](IPConnectionConfiguration/ResyncSequenceNumbers.md) { get; set; } | &gt;Enable/Disable automatic resynchronization of sequence number |
| [SendRepeats](IPConnectionConfiguration/SendRepeats.md) { get; set; } | Defines how often a failed send should be repeated |
| [SendTimeout](IPConnectionConfiguration/SendTimeout.md) { get; set; } | maximum time a send can take before it is timed out |

## See Also

* namespace [Tiveria.Home.Knx.IP.Connections](../Tiveria.Home.Knx.IP.ConnectionsNamespace.md.md)
* assembly [Tiveria.Home.Knx.IP](../Tiveria.Home.Knx.IP.md)
* [IPConnectionConfiguration.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP/Connections/IPConnectionConfiguration.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->