# IUdpClient.ReceiveAsync method (1 of 3)

Returns a UDP datagram asynchronously that was sent by a remote host.

```csharp
public Task<UdpReceiveResult> ReceiveAsync()
```

## Return Value

The task object representing the asynchronous operation.

## See Also

* interface [IUdpClient](../IUdpClient.md)
* namespace [Tiveria.Home.Knx.IP.Extensions](../IUdpClient.md.md)
* assembly [Tiveria.Home.Knx.IP](../../Tiveria.Home.Knx.IP.md)

---

# IUdpClient.ReceiveAsync method (2 of 3)

Returns a UDP datagram asynchronously that was sent by a remote host.

```csharp
public ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
```

| parameter | description |
| --- | --- |
| cancellationToken | The token to monitor for cancellation requests. |

## Return Value

The task object representing the asynchronous operation.

## See Also

* interface [IUdpClient](../IUdpClient.md)
* namespace [Tiveria.Home.Knx.IP.Extensions](../IUdpClient.md.md)
* assembly [Tiveria.Home.Knx.IP](../../Tiveria.Home.Knx.IP.md)

---

# IUdpClient.ReceiveAsync method (3 of 3)

Returns a UDP datagram asynchronously that was sent by a remote host.

```csharp
public ValueTask<UdpReceiveResult> ReceiveAsync(int timeoutMS, 
    CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| timeoutMS | max time to receive data in milliseconds |
| cancellationToken | The token to monitor for cancellation requests. |

## Return Value

The task object representing the asynchronous operation.

## See Also

* interface [IUdpClient](../IUdpClient.md)
* namespace [Tiveria.Home.Knx.IP.Extensions](../IUdpClient.md.md)
* assembly [Tiveria.Home.Knx.IP](../../Tiveria.Home.Knx.IP.md)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->
