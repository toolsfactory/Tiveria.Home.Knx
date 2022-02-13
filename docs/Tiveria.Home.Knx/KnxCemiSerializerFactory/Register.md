# KnxCemiSerializerFactory.Register&lt;T&gt; method

Registers a new [`IKnxCemiSerializer`](../IKnxCemiSerializer.md) based type to de/serialize Cemi messages with a specific [`MessageCode`](../../Tiveria.Home.Knx.Cemi/MessageCode.md)

```csharp
public void Register<T>(MessageCode messageCode)
    where T : IKnxCemiSerializer, new()
```

| parameter | description |
| --- | --- |
| T | The type to register |
| messageCode | The [`MessageCode`](../../Tiveria.Home.Knx.Cemi/MessageCode.md) to register the serializer for |

## See Also

* enum [MessageCode](../../Tiveria.Home.Knx.Cemi/MessageCode.md)
* interface [IKnxCemiSerializer](../IKnxCemiSerializer.md)
* class [KnxCemiSerializerFactory](../KnxCemiSerializerFactory.md)
* namespace [Tiveria.Home.Knx](../KnxCemiSerializerFactory.md.md)
* assembly [Tiveria.Home.Knx](../../Tiveria.Home.Knx.md)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->