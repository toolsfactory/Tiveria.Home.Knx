# IConnectedDeviceManagement.ResetDeviceAsync method

Sends a reset request to a device. Use with care!

```csharp
public Task<(byte ErrorCode, int ProcessTime)> ResetDeviceAsync(EraseCode code, int channelid, 
    bool areYouSure)
```

| parameter | description |
| --- | --- |
| code | The erase code to be used |
| channelid | The channel to be reset |
| areYouSure | A confirmation flag |

## See Also

* enum [EraseCode](../EraseCode.md)
* interface [IConnectedDeviceManagement](../IConnectedDeviceManagement.md)
* namespace [Tiveria.Home.Knx.Management](../IConnectedDeviceManagement.md.md)
* assembly [Tiveria.Home.Knx](../../Tiveria.Home.Knx.md)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->
