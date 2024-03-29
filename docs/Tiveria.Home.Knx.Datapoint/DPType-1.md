# DPType&lt;TValue&gt; class

```csharp
public abstract class DPType<TValue> : IDatapointType
```

## Public Members

| name | description |
| --- | --- |
| [DataSize](DPType-1/DataSize.md) { get; protected set; } |  |
| [Description](DPType-1/Description.md) { get; } |  |
| [Id](DPType-1/Id.md) { get; } |  |
| [Maximum](DPType-1/Maximum.md) { get; } |  |
| [Minimum](DPType-1/Minimum.md) { get; } |  |
| [Name](DPType-1/Name.md) { get; } |  |
| [Unit](DPType-1/Unit.md) { get; } |  |
| virtual [Decode](DPType-1/Decode.md)(…) |  |
| virtual [DecodeObject](DPType-1/DecodeObject.md)(…) |  |
| virtual [DecodeString](DPType-1/DecodeString.md)(…) |  |
| virtual [Encode](DPType-1/Encode.md)(…) |  |
| abstract [Encode](DPType-1/Encode.md)(…) |  |
| [Equals](DPType-1/Equals.md)(…) |  |
| override [Equals](DPType-1/Equals.md)(…) |  |
| override [GetHashCode](DPType-1/GetHashCode.md)() |  |
| virtual [IsMainCategory](DPType-1/IsMainCategory.md)(…) |  |
| [operator ==](DPType-1/op_Equality.md) |  |
| [operator !=](DPType-1/op_Inequality.md) |  |

## Protected Members

| name | description |
| --- | --- |
| [DPType](DPType-1/DPType.md)(…) |  |
| [_mainCategory](DPType-1/_mainCategory.md) |  |
| [_subCategory](DPType-1/_subCategory.md) |  |
| virtual [EncodeFromBool](DPType-1/EncodeFromBool.md)(…) |  |
| virtual [EncodeFromDouble](DPType-1/EncodeFromDouble.md)(…) |  |
| virtual [EncodeFromLong](DPType-1/EncodeFromLong.md)(…) |  |
| virtual [EncodeFromObject](DPType-1/EncodeFromObject.md)(…) |  |
| virtual [EncodeFromString](DPType-1/EncodeFromString.md)(…) |  |
| virtual [EncodeFromULong](DPType-1/EncodeFromULong.md)(…) |  |
| virtual [ValidateMinMax](DPType-1/ValidateMinMax.md)(…) |  |

## See Also

* interface [IDatapointType](./IDatapointType.md)
* namespace [Tiveria.Home.Knx.Datapoint](../Tiveria.Home.Knx.DatapointNamespace.md.md)
* assembly [Tiveria.Home.Knx](../Tiveria.Home.Knx.md)
* [DPType.cs](https://github.com/toolsfactory/Tiveria.Home.Knx/tree/main/src/Tiveria.Home.Knx/Datapoint/DPType.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->
