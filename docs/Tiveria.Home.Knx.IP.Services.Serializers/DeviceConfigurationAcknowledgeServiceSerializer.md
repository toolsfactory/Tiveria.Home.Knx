# DeviceConfigurationAcknowledgeServiceSerializer class

```csharp
+--------+--------+--------+--------+
| byte 7 | byte 8 | byte 9 | byte 10|
+--------+--------+--------+--------+
| Header |Channel |Sequence|Status  |
| Length |ID      |Counter |Code    |
+--------+--------+--------+--------+
| 0x04   |        |        |        |
+--------+--------+-----------------+

Serice Type:  
```

```csharp
public class DeviceConfigurationAcknowledgeServiceSerializer : 
    ServiceSerializerBase<DeviceConfigurationAcknowledgeService>
```

## Public Members

| name | description |
| --- | --- |
| [DeviceConfigurationAcknowledgeServiceSerializer](DeviceConfigurationAcknowledgeServiceSerializer/DeviceConfigurationAcknowledgeServiceSerializer.md)() | The default constructor. |
| override [ServiceTypeIdentifier](DeviceConfigurationAcknowledgeServiceSerializer/ServiceTypeIdentifier.md) { get; } |  |
| override [Deserialize](DeviceConfigurationAcknowledgeServiceSerializer/Deserialize.md)(…) |  |
| override [Serialize](DeviceConfigurationAcknowledgeServiceSerializer/Serialize.md)(…) |  |

## See Also

* class [ServiceSerializerBase&lt;T&gt;](./ServiceSerializerBase-1.md)
* class [DeviceConfigurationAcknowledgeService](../Tiveria.Home.Knx.IP.Services/DeviceConfigurationAcknowledgeService.md)
* namespace [Tiveria.Home.Knx.IP.Services.Serializers](../Tiveria.Home.Knx.IP.Services.SerializersNamespace.md.md)
* assembly [Tiveria.Home.Knx.IP](../Tiveria.Home.Knx.IP.md)
* [DeviceConfigurationAcknowledgeServiceSerializer.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP/Services/Serializers/DeviceConfigurationAcknowledgeServiceSerializer.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->
