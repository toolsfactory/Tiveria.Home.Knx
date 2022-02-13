# ConnectionRequestServiceSerializer class

```csharp
Frame Header not shown here.
+--------+--------+--------+--------+--------+--------+
| byte 7 | byte 8 | byte 9 | byte 10| byte 11| byte 12|
+--------+--------+--------+--------+--------+--------+
| HPAI            | HPAI            | CRI Connection  |
| Control Endpoint| Data Endpoint   | Request Info    |
+-----------------+-----------------+-----------------+
```

```csharp
public class ConnectionRequestServiceSerializer : ServiceSerializerBase<ConnectionRequestService>
```

## Public Members

| name | description |
| --- | --- |
| [ConnectionRequestServiceSerializer](ConnectionRequestServiceSerializer/ConnectionRequestServiceSerializer.md)() | The default constructor. |
| override [ServiceTypeIdentifier](ConnectionRequestServiceSerializer/ServiceTypeIdentifier.md) { get; } |  |
| override [Deserialize](ConnectionRequestServiceSerializer/Deserialize.md)(…) |  |
| override [Serialize](ConnectionRequestServiceSerializer/Serialize.md)(…) |  |

## See Also

* class [ServiceSerializerBase&lt;T&gt;](./ServiceSerializerBase-1.md)
* class [ConnectionRequestService](../Tiveria.Home.Knx.IP.Services/ConnectionRequestService.md)
* namespace [Tiveria.Home.Knx.IP.Services.Serializers](../Tiveria.Home.Knx.IP.Services.SerializersNamespace.md.md)
* assembly [Tiveria.Home.Knx.IP](../Tiveria.Home.Knx.IP.md)
* [ConnectionRequestServiceSerializer.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP/Services/Serializers/ConnectionRequestServiceSerializer.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->