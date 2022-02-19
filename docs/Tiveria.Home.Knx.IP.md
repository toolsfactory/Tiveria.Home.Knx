# Tiveria.Home.Knx.IP assembly

## Tiveria.Home.Knx.IP namespace

| public type | description |
| --- | --- |
| class [FrameReceivedEventArgs](./Tiveria.Home.Knx.IP/FrameReceivedEventArgs.md) |  |
| interface [IKnxNetIPConnection](./Tiveria.Home.Knx.IP/IKnxNetIPConnection.md) | Interface for all IP based connections ot the Knx bus |
| interface [IKnxNetIPFrame](./Tiveria.Home.Knx.IP/IKnxNetIPFrame.md) |  |
| interface [IKnxNetIPService](./Tiveria.Home.Knx.IP/IKnxNetIPService.md) |  |
| interface [IKnxNetIPServiceSerializer&lt;T&gt;](./Tiveria.Home.Knx.IP/IKnxNetIPServiceSerializer-1.md) |  |
| interface [IKnxNetIPServiceSerializer](./Tiveria.Home.Knx.IP/IKnxNetIPServiceSerializer.md) |  |
| delegate [KnxFrameReceivedDelegate](./Tiveria.Home.Knx.IP/KnxFrameReceivedDelegate.md) |  |
| static class [KnxNetIPConstants](./Tiveria.Home.Knx.IP/KnxNetIPConstants.md) |  |
| class [KnxNetIPFrame](./Tiveria.Home.Knx.IP/KnxNetIPFrame.md) |  |
| class [KnxNetIPServiceSerializerFactory](./Tiveria.Home.Knx.IP/KnxNetIPServiceSerializerFactory.md) |  |
| class [KnxNetIPVersion](./Tiveria.Home.Knx.IP/KnxNetIPVersion.md) | Structure representing the KNX Version information |
| delegate [PacketReceivedDelegate](./Tiveria.Home.Knx.IP/PacketReceivedDelegate.md) |  |

## Tiveria.Home.Knx.IP.Connections namespace

| public type | description |
| --- | --- |
| delegate [HeartbeatFailedDelegate](./Tiveria.Home.Knx.IP.Connections/HeartbeatFailedDelegate.md) | Delegate definition used by HeartbeatMonitor to inform owner about a failure |
| class [HeartbeatMonitor](./Tiveria.Home.Knx.IP.Connections/HeartbeatMonitor.md) | Internal class that helps monitoring the connection state of a KNXNetIP tunneling connection |
| delegate [HeartbeatOkDelegate](./Tiveria.Home.Knx.IP.Connections/HeartbeatOkDelegate.md) | Delegate definition used by HeartbeatMonitor to inform owner about a successful beat |
| abstract class [IPConnectionBase](./Tiveria.Home.Knx.IP.Connections/IPConnectionBase.md) | Base class for all ip connections. This clas provides basic shared functionalities and helper functions. |
| record [IPConnectionConfiguration](./Tiveria.Home.Knx.IP.Connections/IPConnectionConfiguration.md) | Configuration options for all kinds of KnxNetIP connections |
| record [KnxNetIPServerDescription](./Tiveria.Home.Knx.IP.Connections/KnxNetIPServerDescription.md) | This record represents a single KnxNetIP router or interface found via the [`KnxNetIPServerDiscoveryAgent`](./Tiveria.Home.Knx.IP.Connections/KnxNetIPServerDiscoveryAgent.md). |
| class [KnxNetIPServerDiscoveryAgent](./Tiveria.Home.Knx.IP.Connections/KnxNetIPServerDiscoveryAgent.md) | Provides means to search for KnxNetIP interfaces and routers in the networks the host is connected with. (Even though not fully correct, KnxNetIP iterfaces and routers are called servers in here) |
| class [RoutingConnection](./Tiveria.Home.Knx.IP.Connections/RoutingConnection.md) | Class for connecting via KnxNetIP Routing to the Knx infrastructure |
| class [ServerRespondedEventArgs](./Tiveria.Home.Knx.IP.Connections/ServerRespondedEventArgs.md) | Arguments class used by [`KnxNetIPServerDiscoveryAgent`](./Tiveria.Home.Knx.IP.Connections/KnxNetIPServerDiscoveryAgent.md) in case a server responded |
| class [TunnelingConnection](./Tiveria.Home.Knx.IP.Connections/TunnelingConnection.md) | CLass allowing to connect to a KNX IP Router or Interface using Tunneling. |
| class [TunnelingConnectionBuilder](./Tiveria.Home.Knx.IP.Connections/TunnelingConnectionBuilder.md) | Builder to simplify creating [`TunnelingConnection`](./Tiveria.Home.Knx.IP.Connections/TunnelingConnection.md) instances. |
| record [TunnelingConnectionConfiguration](./Tiveria.Home.Knx.IP.Connections/TunnelingConnectionConfiguration.md) | Configuration options for a KnxNetIP Tunneling connection |
| class [UdpPacketReceiver](./Tiveria.Home.Knx.IP.Connections/UdpPacketReceiver.md) |  |

## Tiveria.Home.Knx.IP.DeviceManagement namespace

| public type | description |
| --- | --- |
| class [TunnelingManagementClient](./Tiveria.Home.Knx.IP.DeviceManagement/TunnelingManagementClient.md) |  |

## Tiveria.Home.Knx.IP.Enums namespace

| public type | description |
| --- | --- |
| enum [ConnectionState](./Tiveria.Home.Knx.IP.Enums/ConnectionState.md) |  |
| enum [ConnectionType](./Tiveria.Home.Knx.IP.Enums/ConnectionType.md) | Enumeration with all available connection types mapped to the correct byte encoding. |
| enum [DeviceStatus](./Tiveria.Home.Knx.IP.Enums/DeviceStatus.md) |  |
| static class [EnumExtensions](./Tiveria.Home.Knx.IP.Enums/EnumExtensions.md) | Extension methods used to translate the KNXet/IP specific enums to readable strings |
| enum [ErrorCodes](./Tiveria.Home.Knx.IP.Enums/ErrorCodes.md) |  |
| enum [HPAIEndpointType](./Tiveria.Home.Knx.IP.Enums/HPAIEndpointType.md) | Enum with all KNXnet/IP Host Protocol Address Information (Hpai) Endpoit Types. |
| enum [KnxMediumType](./Tiveria.Home.Knx.IP.Enums/KnxMediumType.md) |  |
| static class [ServiceTypeIdentifier](./Tiveria.Home.Knx.IP.Enums/ServiceTypeIdentifier.md) | Enum with all KNXnet/IP Servicetype Identifiers and their correct hex codes |
| enum [SrpType](./Tiveria.Home.Knx.IP.Enums/SrpType.md) | List of Search Request Parameter types |
| enum [TunnelingLayer](./Tiveria.Home.Knx.IP.Enums/TunnelingLayer.md) | Enum with all KNXnet/IP Tunneling Layer Types. |

## Tiveria.Home.Knx.IP.Extensions namespace

| public type | description |
| --- | --- |
| interface [IUdpClient](./Tiveria.Home.Knx.IP.Extensions/IUdpClient.md) | Interface for a facade of the standard UdpClient |
| static class [LoggerExtensions](./Tiveria.Home.Knx.IP.Extensions/LoggerExtensions.md) |  |
| static class [UdpClientFactory](./Tiveria.Home.Knx.IP.Extensions/UdpClientFactory.md) | Static factory class encapsulating [`IUdpClient`](./Tiveria.Home.Knx.IP.Extensions/IUdpClient.md) implementations. Goal is to be able to mock UdpClient |
| class [UdpClientWrapper](./Tiveria.Home.Knx.IP.Extensions/UdpClientWrapper.md) |  |

## Tiveria.Home.Knx.IP.Services namespace

| public type | description |
| --- | --- |
| class [ConnectionRequestService](./Tiveria.Home.Knx.IP.Services/ConnectionRequestService.md) | Frame Header not shown here. +--------+--------+--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; byte 11&#x7C; byte 12&#x7C; +--------+--------+--------+--------+--------+--------+ &#x7C; HPAI &#x7C; HPAI &#x7C; CRI Connection &#x7C; &#x7C; Control Endpoint&#x7C; Data Endpoint &#x7C; Request Info &#x7C; +-----------------+-----------------+-----------------+ |
| class [ConnectionResponseService](./Tiveria.Home.Knx.IP.Services/ConnectionResponseService.md) | Frame Header not shown here. +--------+--------+--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; byte 11&#x7C; byte 12&#x7C; +--------+--------+--------+--------+--------+--------+ &#x7C; Channel&#x7C;Status &#x7C; HPAI &#x7C; CRD Connection &#x7C; &#x7C; Id &#x7C; &#x7C; Data Endpoint &#x7C; Response Data &#x7C; +-----------------+-----------------+-----------------+ |
| class [ConnectionStateRequestService](./Tiveria.Home.Knx.IP.Services/ConnectionStateRequestService.md) | +--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; +--------+--------+--------+--------+ &#x7C; Channel&#x7C;Reserved&#x7C; HPAI &#x7C; &#x7C; ID &#x7C; 0x00 &#x7C; Control Endpoint&#x7C; +--------+--------+-----------------+ |
| class [ConnectionStateResponseService](./Tiveria.Home.Knx.IP.Services/ConnectionStateResponseService.md) | +--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; +--------+--------+ &#x7C; Channel&#x7C;Status &#x7C; &#x7C; ID &#x7C; 0x00 &#x7C; +--------+--------+ |
| class [DescriptionRequestService](./Tiveria.Home.Knx.IP.Services/DescriptionRequestService.md) | +--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; +--------+--------+ &#x7C; HPAI &#x7C; &#x7C; Control Endpoint&#x7C; +--------+--------+ |
| class [DescriptionResponseService](./Tiveria.Home.Knx.IP.Services/DescriptionResponseService.md) |  |
| class [DeviceConfigurationAcknowledgeService](./Tiveria.Home.Knx.IP.Services/DeviceConfigurationAcknowledgeService.md) | +--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; +--------+--------+--------+--------+ &#x7C; Header &#x7C;Channel &#x7C;Sequence&#x7C;Status &#x7C; &#x7C; Length &#x7C;ID &#x7C;Counter &#x7C;Code &#x7C; +--------+--------+--------+--------+ &#x7C; 0x04 &#x7C; &#x7C; &#x7C; &#x7C; +--------+--------+-----------------+ Serice Type: |
| class [DeviceConfigurationRequestService](./Tiveria.Home.Knx.IP.Services/DeviceConfigurationRequestService.md) |  |
| class [DisconnectRequestService](./Tiveria.Home.Knx.IP.Services/DisconnectRequestService.md) | +--------+--------+--------+--------+ &#x7C; byte 1 &#x7C; byte 2 &#x7C; byte 3 &#x7C; byte 4 &#x7C; +--------+--------+--------+--------+ &#x7C; Channel&#x7C;Reserved&#x7C; HPAI &#x7C; &#x7C; ID &#x7C; 0x00 &#x7C; Control Endpoint&#x7C; +--------+--------+-----------------+ |
| class [DisconnectResponseService](./Tiveria.Home.Knx.IP.Services/DisconnectResponseService.md) | +--------+--------+ &#x7C; byte 1 &#x7C; byte 2 &#x7C; +--------+--------+ &#x7C; Channel&#x7C;Status &#x7C; &#x7C; ID &#x7C; 0x00 &#x7C; +--------+--------+ |
| abstract class [EMIServiceBase](./Tiveria.Home.Knx.IP.Services/EMIServiceBase.md) |  |
| class [ExtendedSearchRequestService](./Tiveria.Home.Knx.IP.Services/ExtendedSearchRequestService.md) |  |
| class [RawService](./Tiveria.Home.Knx.IP.Services/RawService.md) |  |
| class [RoutingBusyService](./Tiveria.Home.Knx.IP.Services/RoutingBusyService.md) |  |
| class [RoutingIndicationService](./Tiveria.Home.Knx.IP.Services/RoutingIndicationService.md) |  |
| class [RoutingLostMessageService](./Tiveria.Home.Knx.IP.Services/RoutingLostMessageService.md) |  |
| class [SearchRequestService](./Tiveria.Home.Knx.IP.Services/SearchRequestService.md) |  |
| class [SearchResponseService](./Tiveria.Home.Knx.IP.Services/SearchResponseService.md) |  |
| abstract class [ServiceBase](./Tiveria.Home.Knx.IP.Services/ServiceBase.md) |  |
| class [TunnelingAcknowledgeService](./Tiveria.Home.Knx.IP.Services/TunnelingAcknowledgeService.md) | +--------+--------+--------+--------+ &#x7C; byte 1 &#x7C; byte 2 &#x7C; byte 3 &#x7C; byte 4 &#x7C; +--------+--------+--------+--------+ &#x7C; Header &#x7C;Channel &#x7C;Sequence&#x7C;Status &#x7C; &#x7C; Length &#x7C;ID &#x7C;Counter &#x7C;Code &#x7C; +--------+--------+--------+--------+ &#x7C; 0x04 &#x7C; &#x7C; &#x7C; &#x7C; +--------+--------+-----------------+ Serice Type: |
| class [TunnelingRequestService](./Tiveria.Home.Knx.IP.Services/TunnelingRequestService.md) |  |

## Tiveria.Home.Knx.IP.Services.Serializers namespace

| public type | description |
| --- | --- |
| class [ConnectionRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/ConnectionRequestServiceSerializer.md) | Frame Header not shown here. +--------+--------+--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; byte 11&#x7C; byte 12&#x7C; +--------+--------+--------+--------+--------+--------+ &#x7C; HPAI &#x7C; HPAI &#x7C; CRI Connection &#x7C; &#x7C; Control Endpoint&#x7C; Data Endpoint &#x7C; Request Info &#x7C; +-----------------+-----------------+-----------------+ |
| class [ConnectionResponseServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/ConnectionResponseServiceSerializer.md) | Frame Header not shown here. +--------+--------+--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; byte 11&#x7C; byte 12&#x7C; +--------+--------+--------+--------+--------+--------+ &#x7C; Channel&#x7C;Status &#x7C; HPAI &#x7C; CRD Connection &#x7C; &#x7C; Id &#x7C; &#x7C; Data Endpoint &#x7C; Response Data &#x7C; +-----------------+-----------------+-----------------+ |
| class [ConnectionStateRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/ConnectionStateRequestServiceSerializer.md) | +--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; +--------+--------+--------+--------+ &#x7C; Channel&#x7C;Reserved&#x7C; HPAI &#x7C; &#x7C; ID &#x7C; 0x00 &#x7C; Control Endpoint&#x7C; +--------+--------+-----------------+ |
| class [ConnectionStateResponseServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/ConnectionStateResponseServiceSerializer.md) | +--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; +--------+--------+ &#x7C; Channel&#x7C;Status &#x7C; &#x7C; ID &#x7C; 0x00 &#x7C; +--------+--------+ |
| class [DescriptionRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/DescriptionRequestServiceSerializer.md) | +--------+--------+--------+--------+--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; byte 11&#x7C; byte 12&#x7C; byte 13&#x7C; byte 14&#x7C; +--------+--------+--------+--------+--------+--------+--------+--------+ &#x7C; Size &#x7C;Endpoint&#x7C; Endpoint IP Address &#x7C; Endpoint Port &#x7C; &#x7C; (8) &#x7C; Type &#x7C; &#x7C; &#x7C; +--------+--------+--------+--------+--------+--------+--------+--------+ |
| class [DescriptionResponseServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/DescriptionResponseServiceSerializer.md) | Device Hardware DIB, Supported Families DIB, Other DIB |
| class [DeviceConfigurationAcknowledgeServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/DeviceConfigurationAcknowledgeServiceSerializer.md) | +--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; +--------+--------+--------+--------+ &#x7C; Header &#x7C;Channel &#x7C;Sequence&#x7C;Status &#x7C; &#x7C; Length &#x7C;ID &#x7C;Counter &#x7C;Code &#x7C; +--------+--------+--------+--------+ &#x7C; 0x04 &#x7C; &#x7C; &#x7C; &#x7C; +--------+--------+-----------------+ Serice Type: |
| class [DeviceConfigurationRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/DeviceConfigurationRequestServiceSerializer.md) |  |
| class [DisconnectRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/DisconnectRequestServiceSerializer.md) | +--------+--------+--------+--------+--------+--------+--------+--------+--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; byte 9 &#x7C; byte 10&#x7C; byte 11&#x7C; byte 12&#x7C; byte 13&#x7C; byte 14&#x7C; byte 15&#x7C; byte 16&#x7C; +--------+--------+--------+--------+--------+--------+--------+--------+--------+--------+ &#x7C; Channel&#x7C;Reserved&#x7C; HPAI Controlpoint &#x7C; &#x7C; ID &#x7C; 0x00 +--------+--------+--------+--------+--------+--------+--------+--------+ &#x7C; &#x7C; &#x7C; Size &#x7C;Endpoint&#x7C; Endpoint IP Address &#x7C; Endpoint Port &#x7C; &#x7C; &#x7C; &#x7C; (8) &#x7C; Type &#x7C; &#x7C; &#x7C; +--------+--------+-----------------+--------+--------+--------+--------+--------+--------+ |
| class [DisconnectResponseServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/DisconnectResponseServiceSerializer.md) | +--------+--------+ &#x7C; byte 7 &#x7C; byte 8 &#x7C; +--------+--------+ &#x7C; Channel&#x7C;Status &#x7C; &#x7C; ID &#x7C; 0x00 &#x7C; +--------+--------+ |
| class [ExtendedSearchRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/ExtendedSearchRequestServiceSerializer.md) |  |
| class [RawServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/RawServiceSerializer.md) |  |
| class [RoutingBusyServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/RoutingBusyServiceSerializer.md) |  |
| class [RoutingIndicationServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/RoutingIndicationServiceSerializer.md) |  |
| class [RoutingLostMessageServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/RoutingLostMessageServiceSerializer.md) |  |
| class [SearchRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/SearchRequestServiceSerializer.md) |  |
| class [SearchResponseServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/SearchResponseServiceSerializer.md) |  |
| abstract class [ServiceSerializerBase&lt;T&gt;](./Tiveria.Home.Knx.IP.Services.Serializers/ServiceSerializerBase-1.md) |  |
| class [TunnelingAcknowledgeServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/TunnelingAcknowledgeServiceSerializer.md) | +--------+--------+--------+--------+ &#x7C; byte 1 &#x7C; byte 2 &#x7C; byte 3 &#x7C; byte 4 &#x7C; +--------+--------+--------+--------+ &#x7C; Header &#x7C;Channel &#x7C;Sequence&#x7C;Status &#x7C; &#x7C; Length &#x7C;ID &#x7C;Counter &#x7C;Code &#x7C; +--------+--------+--------+--------+ &#x7C; 0x04 &#x7C; &#x7C; &#x7C; &#x7C; +--------+--------+-----------------+ Serice Type: |
| class [TunnelingRequestServiceSerializer](./Tiveria.Home.Knx.IP.Services.Serializers/TunnelingRequestServiceSerializer.md) |  |

## Tiveria.Home.Knx.IP.Structures namespace

| public type | description |
| --- | --- |
| class [BusyInfo](./Tiveria.Home.Knx.IP.Structures/BusyInfo.md) |  |
| class [ConnectionHeader](./Tiveria.Home.Knx.IP.Structures/ConnectionHeader.md) | Class representing the KnxNetIP ConnectionHeader used to indicate the channelID and sequenceNo when communication via Tunneling with a Knx Interface. This class allows changing its properties as both values are only set moments before the message is sent. |
| class [CRD](./Tiveria.Home.Knx.IP.Structures/CRD.md) |  |
| class [CRDTunnel](./Tiveria.Home.Knx.IP.Structures/CRDTunnel.md) | Immutable representation of the connection response data block (CRD) for a tunneling connection. Official KNX Documentation: "03_04_08 Tunneling v01.05.03 AS.pdf" -&gt; 4.4.4.4 |
| class [CRI](./Tiveria.Home.Knx.IP.Structures/CRI.md) |  |
| class [CRITunnel](./Tiveria.Home.Knx.IP.Structures/CRITunnel.md) | Immutable representation of the connection request information block (CRI) for a tunneling connection. Official KNX Documentation: "03_04_08 Tunneling v01.05.03 AS.pdf" -&gt; 4.4.4.3 |
| class [DeviceInfoDIB](./Tiveria.Home.Knx.IP.Structures/DeviceInfoDIB.md) | Represents the Device Information DIB block described in chapter 7.5.4.2 of the spec 3/8/2 KnxIPNet core. |
| class [FrameHeader](./Tiveria.Home.Knx.IP.Structures/FrameHeader.md) | +--------+--------+--------+--------+--------+--------+ &#x7C; byte 1 &#x7C; byte 2 &#x7C; byte 3 &#x7C; byte 4 &#x7C; byte 5 &#x7C; byte 6 &#x7C; +--------+--------+--------+--------+--------+--------+ &#x7C; Header &#x7C;KNXNETIP&#x7C; Service Type &#x7C; Total Length &#x7C; &#x7C; Length &#x7C;Version &#x7C; &#x7C; &#x7C; +--------+--------+--------+--------+--------+--------+ &#x7C; 0x06 &#x7C; 0x10 &#x7C; &#x7C; &#x7C; +--------+--------+-----------------+--------+--------+ Serice Type: Total Length: Header Length + sizeof(cEMI Frame) |
| class [Hpai](./Tiveria.Home.Knx.IP.Structures/Hpai.md) | Represents the Host Protocol Address information block of the KnxNetIP specification. |
| class [LostMessageInfo](./Tiveria.Home.Knx.IP.Structures/LostMessageInfo.md) |  |
| class [OtherDIB](./Tiveria.Home.Knx.IP.Structures/OtherDIB.md) | Represents the Device Information DIB block described in chapter 7.5.4.2 of the spec 3/8/2 KnxIPNet core. |
| class [ServiceFamiliesDIB](./Tiveria.Home.Knx.IP.Structures/ServiceFamiliesDIB.md) |  |
| class [SRP](./Tiveria.Home.Knx.IP.Structures/SRP.md) |  |

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->
