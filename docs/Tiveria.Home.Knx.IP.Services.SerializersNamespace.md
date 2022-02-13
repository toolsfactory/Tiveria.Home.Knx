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

<!-- DO NOT EDIT: generated by xmldocmd for Tiveria.Home.Knx.IP.dll -->