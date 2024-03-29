# ErrorCodes enumeration

```csharp
public enum ErrorCodes : byte
```

## Values

| name | value | description |
| --- | --- | --- |
| NoError | `0` | Operation successfull. |
| HostProtocolType | `1` | The requested host protocol type is not supported. |
| VersionNotSupported | `2` | The requested host protocol version not supported. |
| SequenceNumber | `4` | The sequence number is out of order. |
| ConnectionId | `33` | The server could not find an active data connection with specified ID. |
| ConnectionType | `34` | The server does not support the requested connection type. |
| ConnectionOption | `35` | The server does not support the requested connection options. |
| NoMoreConnections | `36` | The server could not accept a new connection, maximum reached. |
| DataConnection | `38` | The server detected an error concerning the data connection with the specified ID. |
| KnxConnection | `39` | The server detected an error concerning the KNX subsystem connection with the specified ID. |
| TunnelingLayer | `41` | The server does not support the requested tunneling layer. |

## See Also

* namespace [Tiveria.Home.Knx.IP.Enums](../Tiveria.Home.Knx.IP.EnumsNamespace.md.md)
* assembly [Tiveria.Home.Knx.IP](../Tiveria.Home.Knx.IP.md)
* [ErrorCodes.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP/Enums/ErrorCodes.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->
