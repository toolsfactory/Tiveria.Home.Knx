# Tiveria.Home.Knx assembly

## Tiveria.Home.Knx namespace

| public type | description |
| --- | --- |
| class [CemiReceivedArgs](./Tiveria.Home.Knx/CemiReceivedArgs.md) |  |
| class [ConnectionStateChangedEventArgs](./Tiveria.Home.Knx/ConnectionStateChangedEventArgs.md) |  |
| class [DataReceivedArgs](./Tiveria.Home.Knx/DataReceivedArgs.md) |  |
| interface [IKnxCemiSerializer&lt;T&gt;](./Tiveria.Home.Knx/IKnxCemiSerializer-1.md) |  |
| interface [IKnxCemiSerializer](./Tiveria.Home.Knx/IKnxCemiSerializer.md) | Provides standard capabilities to serialize and deserialize Cemi messages |
| interface [IKnxConnection](./Tiveria.Home.Knx/IKnxConnection.md) | Baseline interface used to connect to the Knx bus |
| interface [IKnxDataElement](./Tiveria.Home.Knx/IKnxDataElement.md) | Baseline interface for all elements of a Knx message |
| class [KnxCemiSerializerFactory](./Tiveria.Home.Knx/KnxCemiSerializerFactory.md) | Factory class used to access all registered serializers for sepcific cemi messages |
| enum [KnxConnectionState](./Tiveria.Home.Knx/KnxConnectionState.md) | Enumeration of the different states a connection to the Knx bus can be in |
| static class [KnxConstants](./Tiveria.Home.Knx/KnxConstants.md) | Typical constants used in the Knx context |
| abstract class [KnxDataElement](./Tiveria.Home.Knx/KnxDataElement.md) | Abstract base class for all data elements |

## Tiveria.Home.Knx.Cemi namespace

| public type | description |
| --- | --- |
| class [AdditionalInformationField](./Tiveria.Home.Knx.Cemi/AdditionalInformationField.md) | Class representing an AdditionalInformationField within a cEMI Frame. For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association |
| enum [AdditionalInfoType](./Tiveria.Home.Knx.Cemi/AdditionalInfoType.md) | Enumeration of all all specified types of [`AdditionalInformationField`](./Tiveria.Home.Knx.Cemi/AdditionalInformationField.md) variants |
| enum [ApciFieldLength](./Tiveria.Home.Knx.Cemi/ApciFieldLength.md) |  |
| class [Apdu](./Tiveria.Home.Knx.Cemi/Apdu.md) |  |
| static class [ApduType](./Tiveria.Home.Knx.Cemi/ApduType.md) |  |
| struct [ApduTypeDetail](./Tiveria.Home.Knx.Cemi/ApduTypeDetail.md) |  |
| enum [BroadcastType](./Tiveria.Home.Knx.Cemi/BroadcastType.md) | Enumeration of all specified BroadcastType Flag values |
| abstract class [CemiBase](./Tiveria.Home.Knx.Cemi/CemiBase.md) | Base class handling the core CEMI Frame structures. For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association |
| class [CemiLData](./Tiveria.Home.Knx.Cemi/CemiLData.md) | Class representing a CEMI Frame for L_Data services. For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association |
| class [CemiRaw](./Tiveria.Home.Knx.Cemi/CemiRaw.md) | Class representing a CEMI Frame for a service that is not fully implemented here. For details please read: "03_06_03_EMI_IMI V01.03.03 AS.PDF" chapter 4 from KNX Association |
| enum [ConfirmType](./Tiveria.Home.Knx.Cemi/ConfirmType.md) | Enumeration of the possible values of the ConfirmType Flag in [`ControlField1`](./Tiveria.Home.Knx.Cemi/ControlField1.md) |
| class [ControlField1](./Tiveria.Home.Knx.Cemi/ControlField1.md) | Represents the Control Field 1 of a cEMI structure. Details can be found in "03_06_03_EMI_IMI V01.03.03 AS.PDF" Chapter 4.1.5.3 |
| class [ControlField2](./Tiveria.Home.Knx.Cemi/ControlField2.md) | Represents the Control Field 2 of a cEMI structure. Details can be found in "03_06_03_EMI_IMI V01.03.03 AS.PDF" Chapter 4.1.5.3 |
| enum [ControlType](./Tiveria.Home.Knx.Cemi/ControlType.md) |  |
| enum [DataMode](./Tiveria.Home.Knx.Cemi/DataMode.md) |  |
| enum [EMIVersion](./Tiveria.Home.Knx.Cemi/EMIVersion.md) |  |
| static class [EnumExtensions](./Tiveria.Home.Knx.Cemi/EnumExtensions.md) |  |
| interface [ICemiMessage](./Tiveria.Home.Knx.Cemi/ICemiMessage.md) | Interface describing standard properties and methods available in all classes representing a Cemi Message |
| enum [MessageCode](./Tiveria.Home.Knx.Cemi/MessageCode.md) | Enumeration of all supported Cemi Message Types and their correct binary representation as flag in [`ControlField1`](./Tiveria.Home.Knx.Cemi/ControlField1.md) |
| enum [PacketType](./Tiveria.Home.Knx.Cemi/PacketType.md) | Packet types supported in TCPI Control Flow |
| enum [Priority](./Tiveria.Home.Knx.Cemi/Priority.md) | Enumeration of all available values for the priority flag in [`ControlField1`](./Tiveria.Home.Knx.Cemi/ControlField1.md) |
| enum [SequenceType](./Tiveria.Home.Knx.Cemi/SequenceType.md) | TCPI Flag to indicate if the TPDU has sequnce information |
| class [Tpci](./Tiveria.Home.Knx.Cemi/Tpci.md) | Class representing the Transport Layer Protocol Control Information |

## Tiveria.Home.Knx.Cemi.Serializers namespace

| public type | description |
| --- | --- |
| class [CemiLDataSerializer](./Tiveria.Home.Knx.Cemi.Serializers/CemiLDataSerializer.md) |  |
| class [CemiRawSerializer](./Tiveria.Home.Knx.Cemi.Serializers/CemiRawSerializer.md) |  |
| abstract class [CemiSerializerBase&lt;T&gt;](./Tiveria.Home.Knx.Cemi.Serializers/CemiSerializerBase-1.md) |  |

## Tiveria.Home.Knx.Datapoint namespace

| public type | description |
| --- | --- |
| struct [ComplexDateTime](./Tiveria.Home.Knx.Datapoint/ComplexDateTime.md) |  |
| static class [DatapointTypesList](./Tiveria.Home.Knx.Datapoint/DatapointTypesList.md) |  |
| enum [DayOfWeek](./Tiveria.Home.Knx.Datapoint/DayOfWeek.md) |  |
| abstract class [DPType&lt;TValue&gt;](./Tiveria.Home.Knx.Datapoint/DPType-1.md) |  |
| class [DPType1](./Tiveria.Home.Knx.Datapoint/DPType1.md) |  |
| class [DPType10](./Tiveria.Home.Knx.Datapoint/DPType10.md) |  |
| class [DPType11](./Tiveria.Home.Knx.Datapoint/DPType11.md) |  |
| class [DPType12](./Tiveria.Home.Knx.Datapoint/DPType12.md) |  |
| class [DPType13](./Tiveria.Home.Knx.Datapoint/DPType13.md) |  |
| class [DPType14](./Tiveria.Home.Knx.Datapoint/DPType14.md) |  |
| class [DPType16](./Tiveria.Home.Knx.Datapoint/DPType16.md) |  |
| class [DPType17](./Tiveria.Home.Knx.Datapoint/DPType17.md) |  |
| class [DPType18](./Tiveria.Home.Knx.Datapoint/DPType18.md) |  |
| class [DPType19](./Tiveria.Home.Knx.Datapoint/DPType19.md) |  |
| class [DPType2](./Tiveria.Home.Knx.Datapoint/DPType2.md) |  |
| class [DPType20](./Tiveria.Home.Knx.Datapoint/DPType20.md) |  |
| class [DPType232](./Tiveria.Home.Knx.Datapoint/DPType232.md) |  |
| class [DPType28](./Tiveria.Home.Knx.Datapoint/DPType28.md) |  |
| class [DPType29](./Tiveria.Home.Knx.Datapoint/DPType29.md) |  |
| class [DPType3](./Tiveria.Home.Knx.Datapoint/DPType3.md) |  |
| class [DPType4](./Tiveria.Home.Knx.Datapoint/DPType4.md) |  |
| class [DPType5](./Tiveria.Home.Knx.Datapoint/DPType5.md) |  |
| class [DPType6](./Tiveria.Home.Knx.Datapoint/DPType6.md) |  |
| class [DPType7](./Tiveria.Home.Knx.Datapoint/DPType7.md) |  |
| class [DPType8](./Tiveria.Home.Knx.Datapoint/DPType8.md) |  |
| class [DPType9](./Tiveria.Home.Knx.Datapoint/DPType9.md) |  |
| class [DPT_SCLOModes](./Tiveria.Home.Knx.Datapoint/DPT_SCLOModes.md) |  |
| interface [IDatapointType](./Tiveria.Home.Knx.Datapoint/IDatapointType.md) |  |
| abstract class [KnxEnum](./Tiveria.Home.Knx.Datapoint/KnxEnum.md) |  |
| enum [SceneControlMode](./Tiveria.Home.Knx.Datapoint/SceneControlMode.md) |  |
| struct [TimeOfDay](./Tiveria.Home.Knx.Datapoint/TimeOfDay.md) |  |

## Tiveria.Home.Knx.Exceptions namespace

| public type | description |
| --- | --- |
| abstract class [KnxBaseException](./Tiveria.Home.Knx.Exceptions/KnxBaseException.md) | Base class for all Knx specific Exceptions |
| class [KnxBufferException](./Tiveria.Home.Knx.Exceptions/KnxBufferException.md) |  |
| class [KnxBufferFieldException](./Tiveria.Home.Knx.Exceptions/KnxBufferFieldException.md) | Exception raised whenever a value for a field with predefined correct values is found that is not matching. |
| class [KnxBufferSizeException](./Tiveria.Home.Knx.Exceptions/KnxBufferSizeException.md) | Exception raised when the raw buffer provided to create a structure from doesn't fit in size |
| class [KnxCommunicationException](./Tiveria.Home.Knx.Exceptions/KnxCommunicationException.md) |  |
| class [KnxConnectionException](./Tiveria.Home.Knx.Exceptions/KnxConnectionException.md) |  |
| class [KnxTimeoutException](./Tiveria.Home.Knx.Exceptions/KnxTimeoutException.md) |  |
| class [KnxTranslationException](./Tiveria.Home.Knx.Exceptions/KnxTranslationException.md) |  |

## Tiveria.Home.Knx.Extensions namespace

| public type | description |
| --- | --- |
| static class [BigEndianBinaryReaderExtensions](./Tiveria.Home.Knx.Extensions/BigEndianBinaryReaderExtensions.md) |  |
| static class [Log](./Tiveria.Home.Knx.Extensions/Log.md) |  |

## Tiveria.Home.Knx.GroupManagement namespace

| public type | description |
| --- | --- |
| class [GroupClient&lt;T&gt;](./Tiveria.Home.Knx.GroupManagement/GroupClient-1.md) | Class simplifying the work with one [`GroupAddress`](./Tiveria.Home.Knx.Primitives/GroupAddress.md) |

## Tiveria.Home.Knx.Management namespace

| public type | description |
| --- | --- |
| class [DeviceConnectionSession](./Tiveria.Home.Knx.Management/DeviceConnectionSession.md) |  |
| enum [EraseCode](./Tiveria.Home.Knx.Management/EraseCode.md) | Erase codes used when sending a master reset restart apci code. !:ApduType.RestartMasterReset_Request |
| static class [EraseCodeExtensions](./Tiveria.Home.Knx.Management/EraseCodeExtensions.md) |  |
| interface [IConnectedDeviceManagement](./Tiveria.Home.Knx.Management/IConnectedDeviceManagement.md) | Interface describing all capabilities a management client class should expose |
| interface [IConnectionlessDeviceManagement](./Tiveria.Home.Knx.Management/IConnectionlessDeviceManagement.md) |  |
| interface [INetworkManagement](./Tiveria.Home.Knx.Management/INetworkManagement.md) |  |
| interface [ITransportLayer](./Tiveria.Home.Knx.Management/ITransportLayer.md) |  |
| abstract class [ManagementClientBase](./Tiveria.Home.Knx.Management/ManagementClientBase.md) | Base implementation used in all transport specific [`IConnectedDeviceManagement`](./Tiveria.Home.Knx.Management/IConnectedDeviceManagement.md) implementations |
| enum [ManagementConnectionState](./Tiveria.Home.Knx.Management/ManagementConnectionState.md) | see Chapter 5.1 of 3.3.4 Transport Layer Communication in Knx Specs |
| static class [PropertyDataType](./Tiveria.Home.Knx.Management/PropertyDataType.md) |  |
| record [PropertyDataTypeDetails](./Tiveria.Home.Knx.Management/PropertyDataTypeDetails.md) |  |
| class [PropertyDescription](./Tiveria.Home.Knx.Management/PropertyDescription.md) | Contains the property description information in a PropertyDescription_Response service. |
| enum [TransportConnectionState](./Tiveria.Home.Knx.Management/TransportConnectionState.md) | see Chapter 5.1 of 3.3.4 Transport Layer Communication in Knx Specs |

## Tiveria.Home.Knx.Primitives namespace

| public type | description |
| --- | --- |
| abstract class [Address](./Tiveria.Home.Knx.Primitives/Address.md) | Baseline class for all Knx address types |
| enum [AddressType](./Tiveria.Home.Knx.Primitives/AddressType.md) | Knx Address Type |
| class [GroupAddress](./Tiveria.Home.Knx.Primitives/GroupAddress.md) | Class representing both a 2-level or a 3-level Group Address |
| enum [GroupAddressStyle](./Tiveria.Home.Knx.Primitives/GroupAddressStyle.md) | Style or mode the [`GroupAddress`](./Tiveria.Home.Knx.Primitives/GroupAddress.md) is used in the Knx Project. |
| class [IndividualAddress](./Tiveria.Home.Knx.Primitives/IndividualAddress.md) | Class representing an individual device address |
| class [SerialNumber](./Tiveria.Home.Knx.Primitives/SerialNumber.md) | Class representing a Knx device serialnumber |

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.dll -->
