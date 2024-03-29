# CemiBase class

Base class handling the core CEMI Frame structures. For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association

```csharp
+--------+--------+-----------------+-----------------+
| byte 1 | byte 2 | byte 3 - n      | n bytes         |
+--------+--------+-----------------+-----------------+
|  Msg   |Add.Info|  Additional     |  Service        |
| Code   | Length |  Information    |  Information    |
+--------+--------+-----------------+-----------------+
```

MsgCode : A code describing the message transported in this cEMI Frame See  for all supported codes and their meaning Add.Info Length : 0x00 = no additional info 0x01-0xfe = number of total bytes in Service Information block 0xff = reserved Additional Information : N repetitions of a Additional Information Fields. See  for more details on this substructure

```csharp
public abstract class CemiBase : ICemiMessage
```

## Public Members

| name | description |
| --- | --- |
| [AdditionalInfoFields](CemiBase/AdditionalInfoFields.md) { get; } |  |
| [AdditionalInfoLength](CemiBase/AdditionalInfoLength.md) { get; protected set; } |  |
| [MessageCode](CemiBase/MessageCode.md) { get; protected set; } |  |
| [Size](CemiBase/Size.md) { get; protected set; } |  |
| abstract [CalculateSize](CemiBase/CalculateSize.md)() |  |
| [ToBytes](CemiBase/ToBytes.md)() |  |
| abstract [ToDescription](CemiBase/ToDescription.md)(…) |  |
| abstract [Write](CemiBase/Write.md)(…) |  |

## Protected Members

| name | description |
| --- | --- |
| [CemiBase](CemiBase/CemiBase.md)(…) |  (2 constructors) |
| [_additionalInfoFields](CemiBase/_additionalInfoFields.md) |  |
| [ParseAdditinalInfoLength](CemiBase/ParseAdditinalInfoLength.md)(…) |  |
| [ParseAdditionalInfo](CemiBase/ParseAdditionalInfo.md)(…) |  |
| virtual [ParseBuffer](CemiBase/ParseBuffer.md)(…) |  |
| [ParseMessageCode](CemiBase/ParseMessageCode.md)(…) |  |
| abstract [ParseServiceInformation](CemiBase/ParseServiceInformation.md)(…) |  |
| abstract [VerifyAdditionalLengthInfo](CemiBase/VerifyAdditionalLengthInfo.md)(…) |  |
| abstract [VerifyBufferSize](CemiBase/VerifyBufferSize.md)(…) |  |
| abstract [VerifyMessageCode](CemiBase/VerifyMessageCode.md)(…) |  |

## See Also

* interface [ICemiMessage](./ICemiMessage.md)
* namespace [Tiveria.Home.Knx.Cemi](../Tiveria.Home.Knx.CemiNamespace.md.md)
* assembly [Tiveria.Home.Knx](../Tiveria.Home.Knx.md)
* [CemiBase.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx/Cemi/CemiBase.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->
