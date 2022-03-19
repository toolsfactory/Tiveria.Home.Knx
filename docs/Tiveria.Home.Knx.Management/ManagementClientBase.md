# ManagementClientBase class

Base implementation used in all transport specific [`IConnectedDeviceManagement`](./IConnectedDeviceManagement.md) implementations

```csharp
public abstract class ManagementClientBase
```

## Public Members

| name | description |
| --- | --- |
| [ExtendedFramesSupported](ManagementClientBase/ExtendedFramesSupported.md) { get; } | Indicates if the devices supports extended frames. |
| [IsConnected](ManagementClientBase/IsConnected.md) { get; } | Shows if the client is currently connected to a device |
| [MaxApduFrameLength](ManagementClientBase/MaxApduFrameLength.md) { get; } | Provides the maximum supported lenght of Apdu frames. In case the device doesnt have this property, 15 is set as default. |
| [Priority](ManagementClientBase/Priority.md) { get; set; } | Priority of the messages sent to the device |
| [RemoteAddress](ManagementClientBase/RemoteAddress.md) { get; set; } | Individual address of the remote knx device |
| [ResponseTimeout](ManagementClientBase/ResponseTimeout.md) { get; set; } | Maximum time the client waits for a response to a request. Timout in milliseconds |
| [State](ManagementClientBase/State.md) { get; } | State of the current connection |
| [ConnectAsync](ManagementClientBase/ConnectAsync.md)() | Initiates the connection to the remote device and reads out the max APDU size from object 0 / property 56. |
| [DisconnectAsync](ManagementClientBase/DisconnectAsync.md)() | Disconnets from the device. The underlying connection to the Knx bus is not closed. |
| [ReadADCAsync](ManagementClientBase/ReadADCAsync.md)(…) |  |
| [ReadAddressAsync](ManagementClientBase/ReadAddressAsync.md)(…) |  |
| [ReadAllAddressesAsync](ManagementClientBase/ReadAllAddressesAsync.md)() |  |
| [ReadAllDomainAddressAsync](ManagementClientBase/ReadAllDomainAddressAsync.md)() |  |
| [ReadDeviceDescriptorAsync](ManagementClientBase/ReadDeviceDescriptorAsync.md)(…) |  |
| [ReadDomainAddressAsync](ManagementClientBase/ReadDomainAddressAsync.md)() |  |
| [ReadDomainAddressesAsync](ManagementClientBase/ReadDomainAddressesAsync.md)(…) |  |
| [ReadIndividualAddressAsync](ManagementClientBase/ReadIndividualAddressAsync.md)() |  |
| [ReadMemoryAsync](ManagementClientBase/ReadMemoryAsync.md)(…) |  |
| [ReadPropertiesAsync](ManagementClientBase/ReadPropertiesAsync.md)(…) |  |
| [ReadPropertyAsync](ManagementClientBase/ReadPropertyAsync.md)(…) | Reads the value of a property from the device |
| [ReadPropertyDescriptionAsync](ManagementClientBase/ReadPropertyDescriptionAsync.md)(…) | Read the description for a specific property from the device |
| [ReadPropertyDescriptionByIndexAsync](ManagementClientBase/ReadPropertyDescriptionByIndexAsync.md)(…) |  |
| [ResetDeviceAsync](ManagementClientBase/ResetDeviceAsync.md)(…) |  |
| [RestartDeviceAsync](ManagementClientBase/RestartDeviceAsync.md)() |  |
| [WriteAddressAsync](ManagementClientBase/WriteAddressAsync.md)(…) |  |
| [WriteDomainAddressAsync](ManagementClientBase/WriteDomainAddressAsync.md)(…) |  (2 methods) |
| [WriteMemoryAsync](ManagementClientBase/WriteMemoryAsync.md)(…) |  |
| [WritePropertyAsync](ManagementClientBase/WritePropertyAsync.md)(…) |  |

## Protected Members

| name | description |
| --- | --- |
| [ManagementClientBase](ManagementClientBase/ManagementClientBase.md)(…) | Base constructor |
| [_connection](ManagementClientBase/_connection.md) |  |
| [_disconnectCTS](ManagementClientBase/_disconnectCTS.md) |  |
| [_incLock](ManagementClientBase/_incLock.md) |  |
| [_list](ManagementClientBase/_list.md) |  |
| [_listLock](ManagementClientBase/_listLock.md) |  |
| [_responseApci](ManagementClientBase/_responseApci.md) |  |
| [BuildRequestCemi](ManagementClientBase/BuildRequestCemi.md)(…) |  |
| [GetMaxApduFrameLength](ManagementClientBase/GetMaxApduFrameLength.md)() |  |
| [GetNextSequenceNumber](ManagementClientBase/GetNextSequenceNumber.md)() |  |
| [InitConnection](ManagementClientBase/InitConnection.md)() |  |
| [SendControlCemiAsync](ManagementClientBase/SendControlCemiAsync.md)(…) |  |
| [SendNumberedDataCemiAsync](ManagementClientBase/SendNumberedDataCemiAsync.md)(…) |  |
| [SendTpciAckFrameAsync](ManagementClientBase/SendTpciAckFrameAsync.md)(…) |  |

## See Also

* namespace [Tiveria.Home.Knx.Management](../Tiveria.Home.Knx.ManagementNamespace.md.md)
* assembly [Tiveria.Home.Knx](../Tiveria.Home.Knx.md)
* [ManagementClientBase.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx/Management/ManagementClientBase.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->