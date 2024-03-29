# ConnectionStateResponseService class

```csharp
+--------+--------+
| byte 7 | byte 8 |
+--------+--------+
| Channel|Status  |
| ID     | 0x00   |
+--------+--------+
```

```csharp
public class ConnectionStateResponseService : ServiceBase
```

## Public Members

| name | description |
| --- | --- |
| [ConnectionStateResponseService](ConnectionStateResponseService/ConnectionStateResponseService.md)(…) |  |
| [ChannelId](ConnectionStateResponseService/ChannelId.md) { get; set; } |  |
| override [ServiceTypeIdentifier](ConnectionStateResponseService/ServiceTypeIdentifier.md) { get; } |  |
| [Status](ConnectionStateResponseService/Status.md) { get; set; } |  |
| static readonly [STRUCTURE_SIZE](ConnectionStateResponseService/STRUCTURE_SIZE.md) |  |

## See Also

* class [ServiceBase](./ServiceBase.md)
* namespace [Tiveria.Home.Knx.IP.Services](../Tiveria.Home.Knx.IP.ServicesNamespace.md.md)
* assembly [Tiveria.Home.Knx.IP](../Tiveria.Home.Knx.IP.md)
* [ConnectionStateResponseService.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP/Services/ConnectionStateResponseService.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->
