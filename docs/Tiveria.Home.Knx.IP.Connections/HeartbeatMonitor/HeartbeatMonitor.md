# HeartbeatMonitor constructor

Creates a new heartbeat monitor

```csharp
public HeartbeatMonitor(IUdpClient udpClient, Hpai endpointHpai, byte channelId, 
    HeartbeatOkDelegate hmOkDel, HeartbeatFailedDelegate hbFailDel)
```

| parameter | description |
| --- | --- |
| udpClient | The udp client to use for sending messages |
| endpointHpai | The knx endpoint information to be sent with the heartbeats |
| channelId | The knx channel id to be sent with the heartbeats |
| hmOkDel | Delegate |
| hbFailDel |  |

## See Also

* interface [IUdpClient](../../Tiveria.Home.Knx.IP.Extensions/IUdpClient.md)
* class [Hpai](../../Tiveria.Home.Knx.IP.Structures/Hpai.md)
* delegate [HeartbeatOkDelegate](../HeartbeatOkDelegate.md)
* delegate [HeartbeatFailedDelegate](../HeartbeatFailedDelegate.md)
* class [HeartbeatMonitor](../HeartbeatMonitor.md)
* namespace [Tiveria.Home.Knx.IP.Connections](../HeartbeatMonitor.md.md)
* assembly [Tiveria.Home.Knx.IP](../../Tiveria.Home.Knx.IP.md)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->