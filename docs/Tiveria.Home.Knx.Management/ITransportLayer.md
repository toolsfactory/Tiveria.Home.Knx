# ITransportLayer interface

```csharp
public interface ITransportLayer
```

## Members

| name | description |
| --- | --- |
| [Address](ITransportLayer/Address.md) { get; } | Individual address of the remote knx device |
| [MaxApduFrameLength](ITransportLayer/MaxApduFrameLength.md) { get; } |  |
| [SeqNoReceive](ITransportLayer/SeqNoReceive.md) { get; } |  |
| [SeqNoSend](ITransportLayer/SeqNoSend.md) { get; } |  |
| [State](ITransportLayer/State.md) { get; } |  |
| event [BroadcastReceived](ITransportLayer/BroadcastReceived.md) |  |
| event [Connected](ITransportLayer/Connected.md) |  |
| event [Disconnected](ITransportLayer/Disconnected.md) |  |
| event [GroupReceived](ITransportLayer/GroupReceived.md) |  |
| event [IndividualReceived](ITransportLayer/IndividualReceived.md) |  |
| [BroadcastAsync](ITransportLayer/BroadcastAsync.md)(…) |  |
| [ConnectAsync](ITransportLayer/ConnectAsync.md)() | Establishes a connection to the specific device using a TPCI connect packet |
| [DisconnectAsync](ITransportLayer/DisconnectAsync.md)() | Disconnects from the device |
| [SendAsync](ITransportLayer/SendAsync.md)(…) |  |

## See Also

* namespace [Tiveria.Home.Knx.Management](../Tiveria.Home.Knx.ManagementNamespace.md.md)
* assembly [Tiveria.Home.Knx](../Tiveria.Home.Knx.md)
* [ITransportLayer.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx/Management/ITransportLayer.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->