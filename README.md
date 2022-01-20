![Logo](./other/logo_title_small.png) 



This little library provides basic KNX communication protocols support together with the bility to encode and deode KNX datapoints. It is designed to run in any .NET 6.0 environment and is running in my home project on a RasPi 3 with Raspian Linux.

[![Build&Publish Packages](https://github.com/toolsfactory/Tiveria.Home.Knx/actions/workflows/publish-flow.yml/badge.svg)](https://github.com/toolsfactory/Tiveria.Home.Knx/actions/workflows/publish-flow.yml)

Currently there are two packages available at NuGet:
* [Tiveria.Home.Knx](https://www.nuget.org/packages/Tiveria.Home.Knx/)
    * Knx basic types
    * Knx cEMI frame and datafield types
    * DPT types
    * anything else that is media agnostic
    * Interfaces for clients
* [Tiveria.Home.Knx.IP](https://www.nuget.org/packages/Tiveria.Home.Knx.IP/)
    * All KnxNetIP frame types supported
    * Routing client implementation
    * Tunneling client impementation
    * Helpers

 `./gradlew build`

Supported Features
--------
* KNXnet/IP
    * Tunneling
    * Routing
    * Busmonitor
* KNX IP
* DPT encoding/decoding 
    * 1.* - Boolean (Switch, Alarm, ..)
    * 2.* - Boolean controlled (Switch, Enable, ..)
    * 3.* - 3 Bit controlled (Blinds, Dimming, ..)
    * 5.* - 8 Bit unsigned (Scaling, Tariff information, ..)
    * 6.* - 8 Bit signed (Percent (8 Bit), Status with mode, ..)
    * 7.* - 16 Bit unsigned (Counter, Time period, ..)
    * 8.* - 16 Bit signed (Counter, Time Delta, ..)
    * 9.* - 16 Bit float (Temperature, Humidity, ..)
    * 10.* - Time
    * 11.* - Date
    * 12.* - 32 Bit unsigned 
    * 13.* - 32 Bit signed (Counter pulses, Active Energy, ..)
    * 14.* - 32 Bit float (Acceleration, Electric charge, ..)
    * 16.* - ASCII string (ISO-8859-1 Latin 1)
    * 17.* - Scene number
    * 18.* - Scene control
    * 19.* - Date with time
    * 20.* - 8 Bit enumeration
    * 28.* - UTF-8 string
    * 232.* - RGB
