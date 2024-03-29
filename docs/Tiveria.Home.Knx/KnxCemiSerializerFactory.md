# KnxCemiSerializerFactory class

Factory class used to access all registered serializers for sepcific cemi messages

```csharp
public class KnxCemiSerializerFactory
```

## Public Members

| name | description |
| --- | --- |
| static [Instance](KnxCemiSerializerFactory/Instance.md) { get; } | Property exposing the single instance of this factory |
| [Create](KnxCemiSerializerFactory/Create.md)(…) | Creates a new instance of a [`IKnxCemiSerializer`](./IKnxCemiSerializer.md) based on the provided [`MessageCode`](../Tiveria.Home.Knx.Cemi/MessageCode.md) |
| [Register&lt;T&gt;](KnxCemiSerializerFactory/Register.md)(…) | Registers a new [`IKnxCemiSerializer`](./IKnxCemiSerializer.md) based type to de/serialize Cemi messages with a specific [`MessageCode`](../Tiveria.Home.Knx.Cemi/MessageCode.md) |

## See Also

* namespace [Tiveria.Home.Knx](../Tiveria.Home.KnxNamespace.md.md)
* assembly [Tiveria.Home.Knx](../Tiveria.Home.Knx.md)
* [KnxCemiSerializerFactory.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx/KnxCemiSerializerFactory.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->
