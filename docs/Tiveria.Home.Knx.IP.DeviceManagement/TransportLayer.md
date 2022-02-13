# TransportLayer class

```csharp
public class TransportLayer : ITransportLayer
```

## Public Members

| name | description |
| --- | --- |
| [TransportLayer](TransportLayer/TransportLayer.md)(…) |  |
| [Address](TransportLayer/Address.md) { get; set; } |  |
| [IsConnected](TransportLayer/IsConnected.md) { get; } |  |
| [MaxApduFrameLength](TransportLayer/MaxApduFrameLength.md) { get; } |  |
| [SeqNoReceive](TransportLayer/SeqNoReceive.md) { get; } |  |
| [SeqNoSend](TransportLayer/SeqNoSend.md) { get; } |  |
| [State](TransportLayer/State.md) { get; } |  |
| event [BroadcastReceived](TransportLayer/BroadcastReceived.md) |  |
| event [Connected](TransportLayer/Connected.md) |  |
| event [Disconnected](TransportLayer/Disconnected.md) |  |
| event [GroupReceived](TransportLayer/GroupReceived.md) |  |
| event [IndividualReceived](TransportLayer/IndividualReceived.md) |  |
| [BroadcastAsync](TransportLayer/BroadcastAsync.md)(…) |  |
| [ConnectAsync](TransportLayer/ConnectAsync.md)() |  |
| [DisconnectAsync](TransportLayer/DisconnectAsync.md)() |  |
| [SendAsync](TransportLayer/SendAsync.md)(…) |  |

## Protected Members

| name | description |
| --- | --- |
| [_disconnectCTS](TransportLayer/_disconnectCTS.md) |  |
| [_incLock](TransportLayer/_incLock.md) |  |
| [GetNextSequenceNumber](TransportLayer/GetNextSequenceNumber.md)() |  |
| [InitConnection](TransportLayer/InitConnection.md)() |  |
| [SendControlCemiAsync](TransportLayer/SendControlCemiAsync.md)(…) |  |
| [SendTpciAckFrameAsync](TransportLayer/SendTpciAckFrameAsync.md)(…) |  |

## See Also

* namespace [Tiveria.Home.Knx.IP.DeviceManagement](../Tiveria.Home.Knx.IP.DeviceManagementNamespace.md.md)
* assembly [Tiveria.Home.Knx.IP](../Tiveria.Home.Knx.IP.md)
* [TransportLayer.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx.IP/DeviceManagement/TransportLayer.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->