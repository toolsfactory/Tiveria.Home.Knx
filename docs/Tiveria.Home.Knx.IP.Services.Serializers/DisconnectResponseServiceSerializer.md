# DisconnectResponseServiceSerializer class

```csharp
+--------+--------+
| byte 7 | byte 8 |
+--------+--------+
| Channel|Status  |
| ID     | 0x00   |
+--------+--------+
```

```csharp
public class DisconnectResponseServiceSerializer : ServiceSerializerBase<DisconnectResponseService>
```

## Public Members

| name | description |
| --- | --- |
| [DisconnectResponseServiceSerializer](DisconnectResponseServiceSerializer/DisconnectResponseServiceSerializer.md)() | The default constructor. |
| override [ServiceTypeIdentifier](DisconnectResponseServiceSerializer/ServiceTypeIdentifier.md) { get; } |  |
| override [Deserialize](DisconnectResponseServiceSerializer/Deserialize.md)(…) |  |
| override [Serialize](DisconnectResponseServiceSerializer/Serialize.md)(…) |  |

## See Also

* class [ServiceSerializerBase&lt;T&gt;](./ServiceSerializerBase-1.md)
* class [DisconnectResponseService](../Tiveria.Home.Knx.IP.Services/DisconnectResponseService.md)
* namespace [Tiveria.Home.Knx.IP.Services.Serializers](../Tiveria.Home.Knx.IP.Services.SerializersNamespace.md.md)
* assembly [Tiveria.Home.Knx.IP](../Tiveria.Home.Knx.IP.md)
* [DisconnectResponseServiceSerializer.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP/Services/Serializers/DisconnectResponseServiceSerializer.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->
